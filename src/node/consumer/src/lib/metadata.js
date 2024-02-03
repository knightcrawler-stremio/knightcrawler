import axios from 'axios';
import { search } from 'google-sr';
import nameToImdb from 'name-to-imdb';
import { cacheWrapImdbId, cacheWrapKitsuId, cacheWrapMetadata } from './cache.js';
import { TorrentType } from './types.js';

const CINEMETA_URL = 'https://v3-cinemeta.strem.io';
const KITSU_URL = 'https://anime-kitsu.strem.fun';
const TIMEOUT = 20000;

export function getMetadata(id, type = TorrentType.SERIES) {
  if (!id) {
    return Promise.reject("no valid id provided");
  }

  const key = Number.isInteger(id) || id.match(/^\d+$/) ? `kitsu:${id}` : id;
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

function _requestMetadata(url) {
  return axios.get(url, { timeout: TIMEOUT })
      .then((response) => {
        const body = response.data;
        if (body && body.meta && (body.meta.imdb_id || body.meta.kitsu_id)) {
          return {
            kitsuId: body.meta.kitsu_id,
            imdbId: body.meta.imdb_id,
            type: body.meta.type,
            title: body.meta.name,
            year: body.meta.year,
            country: body.meta.country,
            genres: body.meta.genres,
            status: body.meta.status,
            videos: (body.meta.videos || [])
                .map((video) => Number.isInteger(video.imdbSeason)
                    ? {
                      name: video.name || video.title,
                      season: video.season,
                      episode: video.episode,
                      imdbSeason: video.imdbSeason,
                      imdbEpisode: video.imdbEpisode
                    }
                    : {
                      name: video.name || video.title,
                      season: video.season,
                      episode: video.episode,
                      kitsuId: video.kitsu_id,
                      kitsuEpisode: video.kitsuEpisode,
                      released: video.released
                    }
                ),
            episodeCount: Object.values((body.meta.videos || [])
                .filter((entry) => entry.season !== 0 && entry.episode !== 0)
                .sort((a, b) => a.season - b.season)
                .reduce((map, next) => {
                  map[next.season] = map[next.season] + 1 || 1;
                  return map;
                }, {})),
            totalCount: body.meta.videos && body.meta.videos
                .filter((entry) => entry.season !== 0 && entry.episode !== 0).length
          };
        } else {
          throw new Error('No search results');
        }
      });
}

export function escapeTitle(title) {
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

export async function getImdbId(info, type) {
    const name = escapeTitle(info.title);
    const year = info.year || (info.date && info.date.slice(0, 4));
    const key = `${name}_${year || 'NA'}_${type}`;
    const query = `${name} ${year || ''} ${type} imdb`;
    const fallbackQuery = `${name} ${type} imdb`;
    const googleQuery = year ? query : fallbackQuery;

    try {
        const imdbId = await cacheWrapImdbId(key,
            () => getIMDbIdFromNameToImdb(name, info.year, type)
        );
        return imdbId && 'tt' + imdbId.replace(/tt0*([1-9][0-9]*)$/, '$1').padStart(7, '0');
    } catch (error) {
        const imdbIdFallback = await getIMDbIdFromGoogle(googleQuery);
        return imdbIdFallback && 'tt' + imdbIdFallback.replace(/tt0*([1-9][0-9]*)$/, '$1').padStart(7, '0');
    }
}

function getIMDbIdFromNameToImdb(name, year, type) {
    return new Promise((resolve, reject) => {
        nameToImdb({ name, year, type }, function(err, res) {
            if (res) {
                resolve(res);
            } else {
                reject(err || new Error('Failed IMDbId search'));
            }
        });
    });
}

async function getIMDbIdFromGoogle(query) {
    try {
        const searchResults = await search({ query: query });
        for (const result of searchResults) {
            if (result.link.includes('imdb.com/title/')) {
                const match = result.link.match(/imdb\.com\/title\/(tt\d+)/);
                if (match) {
                    return match[1];
                }
            }
        }
        return undefined;
    }
    catch (error) {
        throw new Error('Failed to find IMDb ID from Google search');
    }
}

export async function getKitsuId(info) {
  const title = escapeTitle(info.title.replace(/\s\|\s.*/, ''));
  const year = info.year ? ` ${info.year}` : '';
  const season = info.season > 1 ? ` S${info.season}` : '';
  const key = `${title}${year}${season}`;
  const query = encodeURIComponent(key);

  return cacheWrapKitsuId(key,
      () => axios.get(`${KITSU_URL}/catalog/series/kitsu-anime-list/search=${query}.json`, { timeout: 60000 })
          .then((response) => {
            const body = response.data;
            if (body && body.metas && body.metas.length) {
              return body.metas[0].id.replace('kitsu:', '');
            } else {
              throw new Error('No search results');
            }
          }));
}

export async function isEpisodeImdbId(imdbId) {
  if (!imdbId) {
    return false;
  }
  return axios.get(`https://www.imdb.com/title/${imdbId}/`, { timeout: 10000 })
      .then(response => !!(response.data && response.data.includes('video.episode')))
      .catch(() => false);
}