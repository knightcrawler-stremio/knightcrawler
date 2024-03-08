import {TorrentType} from '@enums/torrent_types';
import {ICacheService} from "@interfaces/cache_service";
import {ICinemetaJsonResponse} from "@interfaces/cinemeta_metadata";
import {ICommonVideoMetadata} from "@interfaces/common_video_metadata";
import {IKitsuCatalogJsonResponse} from "@interfaces/kitsu_catalog_metadata";
import {IKitsuJsonResponse} from "@interfaces/kitsu_metadata";
import {IMetaDataQuery} from "@interfaces/metadata_query";
import {IMetadataResponse} from "@interfaces/metadata_response";
import {IMetadataService} from "@interfaces/metadata_service";
import {IMongoRepository} from "@mongo/interfaces/mongo_repository";
import {IocTypes} from "@setup/ioc_types";
import axios from 'axios';
import {ResultTypes, search} from 'google-sr';
import {inject, injectable} from "inversify";
import nameToImdb from 'name-to-imdb';

const CINEMETA_URL = 'https://v3-cinemeta.strem.io';
const KITSU_URL = 'https://anime-kitsu.strem.fun';
const TIMEOUT = 60000;

@injectable()
export class MetadataService implements IMetadataService {
    @inject(IocTypes.ICacheService) private cacheService: ICacheService;
    @inject(IocTypes.IMongoRepository) private mongoRepository: IMongoRepository;

    async getKitsuId(info: IMetaDataQuery): Promise<number | Error> {
        const title = this.escapeTitle(info.title!.replace(/\s\|\s.*/, ''));
        const year = info.year ? ` ${info.year}` : '';
        const season = info.season || 0 > 1 ? ` S${info.season}` : '';
        const key = `${title}${year}${season}`;
        const query = encodeURIComponent(key);

        return this.cacheService.cacheWrapKitsuId(key,
            () => axios.get(`${KITSU_URL}/catalog/series/kitsu-anime-list/search=${query}.json`, {timeout: TIMEOUT})
                .then((response) => {
                    const body = response.data as IKitsuCatalogJsonResponse;
                    if (body && body.metas && body.metas.length) {
                        return body.metas[0].id.replace('kitsu:', '');
                    } else {
                        throw new Error('No search results');
                    }
                }));
    }

    async getImdbId(info: IMetaDataQuery): Promise<string | undefined> {
        const name = this.escapeTitle(info.title!);
        const year = info.year || (info.date && info.date.slice(0, 4));
        const key = `${name}_${year || 'NA'}_${info.type}`;
        const query = `${name} ${year || ''} ${info.type} imdb`;
        const fallbackQuery = `${name} ${info.type} imdb`;
        const googleQuery = year ? query : fallbackQuery;

        const imdbInMongo = await this.mongoRepository.getImdbId(name, info.type, year);

        if (imdbInMongo) {
            return imdbInMongo;
        }

        try {
            const imdbId = await this.cacheService.cacheWrapImdbId(key,
                () => this.getIMDbIdFromNameToImdb(name, info)
            );
            return imdbId && 'tt' + imdbId.replace(/tt0*([1-9][0-9]*)$/, '$1').padStart(7, '0');
        } catch (error) {
            const imdbIdFallback = await this.getIMDbIdFromGoogle(googleQuery);
            return imdbIdFallback && 'tt' + imdbIdFallback.toString().replace(/tt0*([1-9][0-9]*)$/, '$1').padStart(7, '0');
        }
    }

    async getMetadata(query: IMetaDataQuery): Promise<IMetadataResponse | Error> {
        if (!query.id) {
            return Promise.reject("no valid id provided");
        }

        const key = Number.isInteger(query.id) || query.id.toString().match(/^\d+$/) ? `kitsu:${query.id}` : query.id;
        const metaType = query.type === TorrentType.Movie ? TorrentType.Movie : TorrentType.Series;
        const isImdbId = Boolean(key.toString().match(/^tt\d+$/));

        try {
            try {
                return await this.cacheService.cacheWrapMetadata(key.toString(), () => {
                    switch (isImdbId) {
                        case true:
                            return this.requestMetadata(`${CINEMETA_URL}/meta/imdb/${key}.json`, this.handleCinemetaResponse);
                        default:
                            return this.requestMetadata(`${KITSU_URL}/meta/${metaType}/${key}.json`, this.handleKitsuResponse)
                    }
                });
            } catch (e) {
                // try different type in case there was a mismatch
                const otherType = metaType === TorrentType.Movie ? TorrentType.Series : TorrentType.Movie;
                return this.requestMetadata(`${CINEMETA_URL}/meta/${otherType}/${key}.json`, this.handleCinemetaResponse)
            }
        } catch (error) {
            throw new Error(`failed metadata query ${key} due: ${error.message}`);
        }
    }

    async isEpisodeImdbId(imdbId: string | undefined): Promise<boolean> {
        if (!imdbId || !imdbId.toString().match(/^tt\d+$/)) {
            return false;
        }

        try {
            const response = await axios.get(`https://www.imdb.com/title/${imdbId}/`, {timeout: TIMEOUT});
            return response.data.includes('video.episode');
        } catch (error) {
            return false;
        }
    }

    escapeTitle(title: string): string {
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

    private requestMetadata = async (url: string, handler: (body: unknown) => IMetadataResponse): Promise<IMetadataResponse> => {
        try {
            const response = await axios.get(url, {timeout: TIMEOUT});
            const body = response.data;
            return handler(body);
        } catch (error) {
            throw new Error(`HTTP error! status: ${error.response?.status}`);
        }
    };

    private handleCinemetaResponse = (response: unknown): IMetadataResponse => {
        const body = response as ICinemetaJsonResponse

        return ({
            imdbId: parseInt(body.meta?.id || '0'),
            type: body.meta?.type,
            title: body.meta?.name,
            year: parseInt(body.meta?.year || '0'),
            country: body.meta?.country,
            genres: body.meta?.genres,
            status: body.meta?.status,
            videos: body.meta?.videos
                ? body.meta.videos.map(video => ({
                    name: video.name,
                    season: video.season,
                    episode: video.episode,
                    imdbSeason: video.season,
                    imdbEpisode: video.episode,
                }))
                : [],
            episodeCount: body.meta?.videos
                ? this.getEpisodeCount(body.meta.videos)
                : [],
            totalCount: body.meta?.videos
                ? body.meta.videos.filter(
                    entry => entry.season !== 0 && entry.episode !== 0
                ).length
                : 0,
        });
    };

    private handleKitsuResponse = (response: unknown): IMetadataResponse => {
        const body = response as IKitsuJsonResponse;

        return ({
            kitsuId: parseInt(body.meta?.kitsu_id || '0'),
            type: body.meta?.type,
            title: body.meta?.name,
            year: parseInt(body.meta?.year || '0'),
            country: body.meta?.country,
            genres: body.meta?.genres,
            status: body.meta?.status,
            videos: body.meta?.videos
                ? body.meta?.videos.map(video => ({
                    name: video.title,
                    season: video.season,
                    episode: video.episode,
                    kitsuId: video.id,
                    kitsuEpisode: video.episode,
                    released: video.released,
                }))
                : [],
            episodeCount: body.meta?.videos
                ? this.getEpisodeCount(body.meta.videos)
                : [],
            totalCount: body.meta?.videos
                ? body.meta.videos.filter(
                    entry => entry.season !== 0 && entry.episode !== 0
                ).length
                : 0,
        });
    };

    private getEpisodeCount = (videos: ICommonVideoMetadata[]): number[] =>
        Object.values(
            videos
                .filter(entry => entry.season !== null && entry.season !== 0 && entry.episode !== 0)
                .sort((a, b) => (a.season || 0) - (b.season || 0))
                .reduce((map: Record<number, number>, next) => {
                    if (next.season || next.season === 0) {
                        map[next.season] = (map[next.season] || 0) + 1;
                    }
                    return map;
                }, {})
        );

    private getIMDbIdFromNameToImdb = (name: string, info: IMetaDataQuery): Promise<string | Error> => {
        const {year} = info;
        const {type} = info;
        return new Promise((resolve, reject) => {
            nameToImdb({name, year, type}, function (err: Error, res: string) {
                if (res) {
                    resolve(res);
                } else {
                    reject(err || new Error('Failed IMDbId search'));
                }
            });
        });
    };

    private getIMDbIdFromGoogle = async (query: string): Promise<string | undefined> => {
        try {
            const searchResults = await search({query: query});
            for (const result of searchResults) {
                if (result.type === ResultTypes.SearchResult) {
                    if (result.link.includes('imdb.com/title/')) {
                        const match = result.link.match(/imdb\.com\/title\/(tt\d+)/);
                        if (match) {
                            return match[1];
                        }
                    }
                }
            }
            return undefined;
        } catch (error) {
            throw new Error('Failed to find IMDb ID from Google search');
        }
    };
}
