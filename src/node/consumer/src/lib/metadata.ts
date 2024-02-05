import axios, {AxiosResponse} from 'axios';
import {search, ResultTypes} from 'google-sr';
import nameToImdb from 'name-to-imdb';
import { cacheWrapImdbId, cacheWrapKitsuId, cacheWrapMetadata } from './cache.js';
import { TorrentType } from './enums/torrent_types';
import {MetadataResponse} from "./interfaces/metadata_response";
import {CinemetaJsonResponse} from "./interfaces/cinemeta_metadata";
import {CommonVideoMetadata} from "./interfaces/common_video_metadata";
import {KitsuJsonResponse} from "./interfaces/kitsu_metadata";
import {MetaDataQuery} from "./interfaces/metadata_query";
import {KitsuCatalogJsonResponse} from "./interfaces/kitsu_catalog_metadata";

const CINEMETA_URL = 'https://v3-cinemeta.strem.io';
const KITSU_URL = 'https://anime-kitsu.strem.fun';
const TIMEOUT = 20000;

async function _requestMetadata(url: string): Promise<MetadataResponse> {
    let response: AxiosResponse<any, any> = await axios.get(url, {timeout: TIMEOUT});
    let result : MetadataResponse;
    const body = response.data;
    if ('kitsu_id' in body.meta) {
        result = handleKitsuResponse(body as KitsuJsonResponse);
    }
    else if ('imdb_id' in body.meta) {
        result = handleCinemetaResponse(body as CinemetaJsonResponse);
    }
    else {
        throw new Error('No valid metadata');
    }
    
    return result;
}

function handleCinemetaResponse(body: CinemetaJsonResponse) : MetadataResponse {
    return {
        imdbId: parseInt(body.meta.imdb_id),
        type: body.meta.type,
        title: body.meta.name,
        year: parseInt(body.meta.year),
        country: body.meta.country,
        genres: body.meta.genres,
        status: body.meta.status,
        videos: body.meta.videos
            ? body.meta.videos.map(video => ({
                name: video.name,
                season: video.season,
                episode: video.episode,
                imdbSeason: video.season,
                imdbEpisode: video.episode,
            }))
            : [],
        episodeCount: body.meta.videos
            ? getEpisodeCount(body.meta.videos)
            : [],
        totalCount: body.meta.videos
            ? body.meta.videos.filter(
                entry => entry.season !== 0 && entry.episode !== 0
            ).length
            : 0,
    };
}

function handleKitsuResponse(body: KitsuJsonResponse) : MetadataResponse {
    return {
        kitsuId: parseInt(body.meta.kitsu_id),
        type: body.meta.type,
        title: body.meta.name,
        year: parseInt(body.meta.year),
        country: body.meta.country,
        genres: body.meta.genres,
        status: body.meta.status,
        videos: body.meta.videos
            ? body.meta.videos.map(video => ({
                name: video.title,
                season: video.season,
                episode: video.episode,
                kitsuId: video.id,
                kitsuEpisode: video.episode,
                released: video.released,
            }))
            : [],
        episodeCount: body.meta.videos
            ? getEpisodeCount(body.meta.videos)
            : [],
        totalCount: body.meta.videos
            ? body.meta.videos.filter(
                entry => entry.season !== 0 && entry.episode !== 0
            ).length
            : 0,
    };
}

function getEpisodeCount(videos: CommonVideoMetadata[]) {
    return Object.values(
        videos
            .filter(entry => entry.season !== 0 && entry.episode !== 0)
            .sort((a, b) => a.season - b.season)
            .reduce((map, next) => {
                map[next.season] = map[next.season] + 1 || 1;
                return map;
            }, {})
    );
}


export function escapeTitle(title: string): string {
  return title.toLowerCase()
      .normalize('NFKD') // normalize non-ASCII characters
      .replace(/[\u0300-\u036F]/g, '')
      .replace(/&/g, 'and')
      .replace(/[;, ~./]+/g, ' ') // replace dots, commas or underscores with spaces
      .replace(/[^\w \-()×+#@!'\u0400-\u04ff]+/g, '') // remove all non-alphanumeric chars
      .replace(/^\d{1,2}[.#\s]+(?=(?:\d+[.\s]*)?[\u0400-\u04ff])/i, '') // remove russian movie numbering
      .replace(/\s{2,}/, ' ') // replace multiple spaces
      .trim();
}

function getIMDbIdFromNameToImdb(name: string, info: MetaDataQuery) : Promise<string | Error> {
    const year = info.year;
    const type = info.type;
    return new Promise((resolve, reject) => {
        nameToImdb({ name, year, type }, function(err: Error, res: string) {
            if (res) {
                resolve(res);
            } else {
                reject(err || new Error('Failed IMDbId search'));
            }
        });
    });
}

async function getIMDbIdFromGoogle(query: string): Promise<string | undefined>{
    try {
        const searchResults = await search({ query: query });
        for(const result of searchResults) {
            if(result.type === ResultTypes.SearchResult) {
                if(result.link.includes('imdb.com/title/')){
                    const match = result.link.match(/imdb\.com\/title\/(tt\d+)/);
                    if(match){
                        return match[1];
                    }
                }
            }
        }
        return undefined;
    }
    catch (error) {
        throw new Error('Failed to find IMDb ID from Google search');
    }
}

export async function getKitsuId(info: MetaDataQuery): Promise<string | Error> {
  const title = escapeTitle(info.title.replace(/\s\|\s.*/, ''));
  const year = info.year ? ` ${info.year}` : '';
  const season = info.season > 1 ? ` S${info.season}` : '';
  const key = `${title}${year}${season}`;
  const query = encodeURIComponent(key);

  return cacheWrapKitsuId(key,
      () => axios.get(`${KITSU_URL}/catalog/series/kitsu-anime-list/search=${query}.json`, { timeout: 60000 })
          .then((response) => {
            const body = response.data as KitsuCatalogJsonResponse;
            if (body && body.metas && body.metas.length) {
              return body.metas[0].id.replace('kitsu:', '');
            } else {
              throw new Error('No search results');
            }
          }));
}

export async function getImdbId(info: MetaDataQuery): Promise<string | undefined> {
    const name = escapeTitle(info.title);
    const year = info.year || (info.date && info.date.slice(0, 4));
    const key = `${name}_${year || 'NA'}_${info.type}`;
    const query = `${name} ${year || ''} ${info.type} imdb`;
    const fallbackQuery = `${name} ${info.type} imdb`;
    const googleQuery = year ? query : fallbackQuery;

    try {
        const imdbId = await cacheWrapImdbId(key,
            () => getIMDbIdFromNameToImdb(name, info)
        );
        return imdbId && 'tt' + imdbId.replace(/tt0*([1-9][0-9]*)$/, '$1').padStart(7, '0');
    } catch (error) {
        const imdbIdFallback = await getIMDbIdFromGoogle(googleQuery);
        return imdbIdFallback && 'tt' + imdbIdFallback.toString().replace(/tt0*([1-9][0-9]*)$/, '$1').padStart(7, '0');
    }
}

export function getMetadata(id: string | number, type: TorrentType = TorrentType.SERIES): Promise<MetadataResponse | Error> {
    if (!id) {
        return Promise.reject("no valid id provided");
    }

    const key = Number.isInteger(id) || id.toString().match(/^\d+$/) ? `kitsu:${id}` : id;
    const metaType = type === TorrentType.MOVIE ? TorrentType.MOVIE : TorrentType.SERIES;
    return cacheWrapMetadata(key, () => _requestMetadata(`${KITSU_URL}/meta/${metaType}/${key}.json`)
        .catch(() => _requestMetadata(`${CINEMETA_URL}/meta/${metaType}/${key}.json`))
        .catch(() => {
            // try different type in case there was a mismatch
            const otherType = metaType === TorrentType.MOVIE ? TorrentType.SERIES : TorrentType.MOVIE;
            return _requestMetadata(`${CINEMETA_URL}/meta/${otherType}/${key}.json`)
        })
        .catch((error) => {
            throw new Error(`failed metadata query ${key} due: ${error.message}`);
        }));
}

export async function isEpisodeImdbId(imdbId: string | undefined): Promise<boolean> {
  if (!imdbId) {
    return false;
  }
  return axios.get(`https://www.imdb.com/title/${imdbId}/`, { timeout: 10000 })
      .then(response => !!(response.data && response.data.includes('video.episode')))
      .catch(() => false);
}