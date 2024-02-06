import Bottleneck from 'bottleneck';
import moment from 'moment';
import { parse } from 'parse-torrent-title';
import {PromiseHelpers} from '../helpers/promises_helpers';
import { TorrentType } from '../enums/torrent_types';
import {TorrentInfo} from "../interfaces/torrent_info";
import { configurationService } from './configuration_service';
import { extensionService } from './extension_service';
import { metadataService } from './metadata_service';
import { parsingService } from './parsing_service';
import { torrentDownloadService } from "./torrent_download_service";
import {logger} from "./logging_service";
import {MetadataResponse} from "../interfaces/metadata_response";
import {ParseTorrentTitleResult} from "../interfaces/parse_torrent_title_result";
import {MetaDataQuery} from "../interfaces/metadata_query";
import {ParsableTorrentFile} from "../interfaces/parsable_torrent_file";
import {CommonVideoMetadata} from "../interfaces/common_video_metadata";
import {TorrentFileCollection} from "../interfaces/torrent_file_collection";

class TorrentFileService {
    private readonly MIN_SIZE: number = 5 * 1024 * 1024; // 5 MB
    private readonly imdb_limiter: Bottleneck = new Bottleneck({
        maxConcurrent: configurationService.metadataConfig.IMDB_CONCURRENT,
        minTime: configurationService.metadataConfig.IMDB_INTERVAL_MS
    });

    public async parseTorrentFiles(torrent: TorrentInfo) {
        const parsedTorrentName = parse(torrent.title);
        const query: MetaDataQuery = {
            id: torrent.kitsuId || torrent.imdbId,
            type: torrent.type || TorrentType.Movie,
        };
        const metadata = await metadataService.getMetadata(query)
            .then(meta => Object.assign({}, meta))
            .catch(() => undefined);

        if (torrent.type !== TorrentType.Anime && metadata && metadata.type && metadata.type !== torrent.type) {
            // it's actually a movie/series
            torrent.type = metadata.type;
        }

        if (torrent.type === TorrentType.Movie && (!parsedTorrentName.seasons ||
            parsedTorrentName.season === 5 && [1, 5].includes(parsedTorrentName.episode))) {
            return this.parseMovieFiles(torrent, metadata);
        }

        return this.parseSeriesFiles(torrent, metadata)
    }

    private async parseMovieFiles(torrent: TorrentInfo, metadata: MetadataResponse): Promise<TorrentFileCollection> {
        const {contents, videos, subtitles} = await this.getMoviesTorrentContent(torrent);
        const filteredVideos = videos
            .filter(video => video.size > this.MIN_SIZE)
            .filter(video => !this.isFeaturette(video));
        if (this.isSingleMovie(filteredVideos)) {
            const parsedVideos = filteredVideos.map(video => ({
                infoHash: torrent.infoHash,
                fileIndex: video.fileIndex,
                title: video.path || torrent.title,
                size: video.size || torrent.size,
                imdbId: torrent.imdbId || metadata && metadata.imdbId,
                kitsuId: torrent.kitsuId || metadata && metadata.kitsuId
            }));
            return {contents, videos: parsedVideos, subtitles};
        }

        const parsedVideos = await PromiseHelpers.sequence(filteredVideos.map(video => () => this.isFeaturette(video)
            ? Promise.resolve(video)
            : this.findMovieImdbId(video.name).then(imdbId => ({...video, imdbId}))))
            .then(videos => videos.map(video => ({
                infoHash: torrent.infoHash,
                fileIndex: video.fileIndex,
                title: video.path || video.name,
                size: video.size,
                imdbId: video.imdbId,
            })));
        return {contents, videos: parsedVideos, subtitles};
    }

    private async parseSeriesFiles(torrent: TorrentInfo, metadata: MetadataResponse) {
        const {contents, videos, subtitles} = await this.getSeriesTorrentContent(torrent);
        const parsedVideos = await Promise.resolve(videos)
            .then(videos => videos.filter(video => videos.length === 1 || video.size > this.MIN_SIZE))
            .then(videos => parsingService.parseSeriesVideos(torrent, videos))
            .then(videos => this.decomposeEpisodes(torrent, videos, metadata))
            .then(videos => this.assignKitsuOrImdbEpisodes(torrent, videos, metadata))
            .then(videos => Promise.all(videos.map(video => video.isMovie
                ? this.mapSeriesMovie(video, torrent)
                : this.mapSeriesEpisode(video, torrent, videos))))
            .then(videos => videos
                .map((video: ParsableTorrentFile) => this.isFeaturette(video) ? this.clearInfoFields(video) : video))
        return {contents, videos: parsedVideos, subtitles};
    }

    private async getMoviesTorrentContent(torrent: TorrentInfo) {
        const files = await torrentDownloadService.getTorrentFiles(torrent)
            .catch(error => {
                if (!parsingService.isPackTorrent(torrent)) {
                    return { videos: [{name: torrent.title, path: torrent.title, size: torrent.size, fileIndex: null}], contents:[], subtitles: []}
                }
                return Promise.reject(error);
            });
        
        if (files.contents && files.contents.length && !files.videos.length && this.isDiskTorrent(files.contents)) {
            files.videos = [{name: torrent.title, path: torrent.title, size: torrent.size}];
        }
        return files;
    }

    private async getSeriesTorrentContent(torrent: TorrentInfo) {
        return torrentDownloadService.getTorrentFiles(torrent)
            .catch(error => {
                if (!parsingService.isPackTorrent(torrent)) {
                    return { videos: [{ name: torrent.title, path: torrent.title, size: torrent.size }], subtitles: [], contents: [] }
                }
                return Promise.reject(error);
            });
    }

    private async mapSeriesEpisode(file: ParsableTorrentFile, torrent: TorrentInfo, files: ParsableTorrentFile[]) : Promise<ParsableTorrentFile> {
        if (!file.episodes && !file.episodes) {
            if (files.length === 1 || files.some(f => f.episodes || f.episodes) || parse(torrent.title).seasons) {
                return Promise.resolve({
                    infoHash: torrent.infoHash,
                    fileIndex: file.fileIndex,
                    title: file.path || file.name,
                    size: file.size,
                    imdbId: torrent.imdbId || file.imdbId,
                });
            }
            return Promise.resolve([]);
        }
        const episodeIndexes = [...(file.episodes || file.episodes).keys()];
        return Promise.resolve(episodeIndexes.map((index) => ({
            infoHash: torrent.infoHash,
            fileIndex: file.fileIndex,
            title: file.path || file.name,
            size: file.size,
            imdbId: file.imdbId || torrent.imdbId,
            season: file.season,
            episode: file.episodes && file.episodes[index],
            episodes: file.episodes,
            kitsuId: file.kitsuId || torrent.kitsuId,
        })))
    }

    private async mapSeriesMovie(file: ParsableTorrentFile, torrent: TorrentInfo): Promise<ParsableTorrentFile> {
        const kitsuId= torrent.type === TorrentType.Anime ? await this.findMovieKitsuId(file)
            .then(result => {
                if (result instanceof Error) {
                    logger.warn(`Failed to retrieve kitsuId due to error: ${result.message}`);
                    return undefined;
                }
                return result;
            }): undefined;
                
        const imdbId = !kitsuId ? await this.findMovieImdbId(file) : undefined;

        const query: MetaDataQuery = {
            id: kitsuId || imdbId,
            type: TorrentType.Movie
        };
        
        const metadataOrError = await metadataService.getMetadata(query);
        if (metadataOrError instanceof Error) {
            logger.warn(`Failed to retrieve metadata due to error: ${metadataOrError.message}`);
            // return default result or throw error, depending on your use case
            return [{
                infoHash: torrent.infoHash,
                fileIndex: file.fileIndex,
                title: file.path || file.name,
                size: file.size,
                imdbId: imdbId,
                kitsuId: kitsuId,
                episodes: undefined,
                imdbSeason: undefined,
                imdbEpisode: undefined,
                kitsuEpisode: undefined
            }];
        }
        // at this point, TypeScript infers that metadataOrError is actually MetadataResponse
        const metadata = metadataOrError;
        const hasEpisode = metadata.videos && metadata.videos.length && (file.episode || metadata.videos.length === 1);
        const episodeVideo = hasEpisode && metadata.videos[(file.episode || 1) - 1];
        return [{
            infoHash: torrent.infoHash,
            fileIndex: file.fileIndex,
            title: file.path || file.name,
            size: file.size,
            imdbId: metadata.imdbId || imdbId,
            kitsuId: metadata.kitsuId || kitsuId,
            season: episodeVideo && metadata.imdbId ? episodeVideo.season : undefined,
            episode: episodeVideo && metadata.imdbId | metadata.kitsuId ? episodeVideo.episode || episodeVideo.episode : undefined,
        }];
    }

    private async decomposeEpisodes(torrent: TorrentInfo, files: ParsableTorrentFile[], metadata: MetadataResponse = { episodeCount: [] }) {
        if (files.every(file => !file.episodes && !file.date)) {
            return files;
        }

        this.preprocessEpisodes(files);

        if (torrent.type === TorrentType.Anime && torrent.kitsuId) {
            if (this.needsCinemetaMetadataForAnime(files, metadata)) {
                // In some cases anime could be resolved to wrong kitsuId
                // because of imdb season naming/absolute per series naming/multiple seasons
                // So in these cases we need to fetch cinemeta based metadata and decompose episodes using that
                await this.updateToCinemetaMetadata(metadata);
                if (files.some(file => Number.isInteger(file.season))) {
                    // sometimes multi season anime torrents don't include season 1 naming
                    files
                        .filter(file => !Number.isInteger(file.season) && file.episodes)
                        .forEach(file => file.season = 1);
                }
            } else {
                // otherwise for anime type episodes are always absolute and for a single season
                files
                    .filter(file => file.episodes && file.season !== 0)
                    .forEach(file => file.season = 1);
                return files;
            }
        }

        const sortedEpisodes = files
            .map(file => !file.isMovie && file.episodes || [])
            .reduce((a, b) => a.concat(b), [])
            .sort((a, b) => a - b);

        if (this.isConcatSeasonAndEpisodeFiles(files, sortedEpisodes, metadata)) {
            this.decomposeConcatSeasonAndEpisodeFiles(files, metadata);
        } else if (this.isDateEpisodeFiles(files, metadata)) {
            this.decomposeDateEpisodeFiles(files, metadata);
        } else if (this.isAbsoluteEpisodeFiles(torrent, files, metadata)) {
            this.decomposeAbsoluteEpisodeFiles(torrent, files, metadata);
        }
        // decomposeEpisodeTitleFiles(torrent, files, metadata);

        return files;
    }

    private preprocessEpisodes(files: ParsableTorrentFile[]) {
        // reverse special episode naming when they named with 0 episode, ie. S02E00
        files
            .filter(file => Number.isInteger(file.season) && file.episode === 0)
            .forEach(file => {
                file.episode = file.season
                file.episodes = [file.season]
                file.season = 0;
            })
    }

    private isConcatSeasonAndEpisodeFiles(files: ParsableTorrentFile[], sortedEpisodes: number[], metadata: MetadataResponse) {
        if (metadata.kitsuId !== undefined) {
            // anime does not use this naming scheme in 99% of cases;
            return false;
        }
        // decompose concat season and episode files (ex. 101=S01E01) in case:
        // 1. file has a season, but individual files are concatenated with that season (ex. path Season 5/511 - Prize
        // Fighters.avi)
        // 2. file does not have a season and the episode does not go out of range for the concat season
        // episode count
        const thresholdAbove = Math.max(Math.ceil(files.length * 0.05), 5);
        const thresholdSorted = Math.max(Math.ceil(files.length * 0.8), 8);
        const threshold = Math.max(Math.ceil(files.length * 0.8), 5);
        const sortedConcatEpisodes = sortedEpisodes
            .filter(ep => ep > 100)
            .filter(ep => metadata.episodeCount[this.div100(ep) - 1] < ep)
            .filter(ep => metadata.episodeCount[this.div100(ep) - 1] >= this.mod100(ep));
        const concatFileEpisodes = files
            .filter(file => !file.isMovie && file.episodes)
            .filter(file => !file.season || file.episodes.every(ep => this.div100(ep) === file.season));
        const concatAboveTotalEpisodeCount = files
            .filter(file => !file.isMovie && file.episodes && file.episodes.every(ep => ep > 100))
            .filter(file => file.episodes.every(ep => ep > metadata.totalCount));
        return sortedConcatEpisodes.length >= thresholdSorted && concatFileEpisodes.length >= threshold
            || concatAboveTotalEpisodeCount.length >= thresholdAbove;
    }

    private isDateEpisodeFiles(files: ParsableTorrentFile[], metadata: MetadataResponse) {
        return files.every(file => (!file.season || !metadata.episodeCount[file.season - 1]) && file.date);
    }

    private isAbsoluteEpisodeFiles(torrent: TorrentInfo, files: ParsableTorrentFile[], metadata: MetadataResponse) {
        const threshold = Math.ceil(files.length / 5);
        const isAnime = torrent.type === TorrentType.Anime && torrent.kitsuId;
        const nonMovieEpisodes = files
            .filter(file => !file.isMovie && file.episodes);
        const absoluteEpisodes = files
            .filter(file => file.season && file.episodes)
            .filter(file => file.episodes.every(ep => metadata.episodeCount[file.season - 1] < ep))
        return nonMovieEpisodes.every(file => !file.season)
            || (isAnime && nonMovieEpisodes.every(file => file.season > metadata.episodeCount.length))
            || absoluteEpisodes.length >= threshold;
    }

    private isNewEpisodeNotInMetadata(torrent: TorrentInfo, file: ParsableTorrentFile, metadata: MetadataResponse) {
        // new episode might not yet been indexed by cinemeta.
        // detect this if episode number is larger than the last episode or season is larger than the last one
        // only for non anime metas
        const isAnime = torrent.type === TorrentType.Anime && torrent.kitsuId;
        return !isAnime && !file.isMovie && file.episodes && file.season !== 1
            && /continuing|current/i.test(metadata.status)
            && file.season >= metadata.episodeCount.length
            && file.episodes.every(ep => ep > (metadata.episodeCount[file.season - 1] || 0));
    }

    private decomposeConcatSeasonAndEpisodeFiles(files: ParsableTorrentFile[], metadata: MetadataResponse) {
        files
            .filter(file => file.episodes && file.season !== 0 && file.episodes.every(ep => ep > 100))
            .filter(file => metadata.episodeCount[(file.season || this.div100(file.episodes[0])) - 1] < 100)
            .filter(file => file.season && file.episodes.every(ep => this.div100(ep) === file.season) || !file.season)
            .forEach(file => {
                file.season = this.div100(file.episodes[0]);
                file.episodes = file.episodes.map(ep => this.mod100(ep))
            });

    }

    private decomposeAbsoluteEpisodeFiles(torrent: TorrentInfo, files: ParsableTorrentFile[], metadata: MetadataResponse) {
        if (metadata.episodeCount.length === 0) {
            files
                .filter(file => !Number.isInteger(file.season) && file.episodes && !file.isMovie)
                .forEach(file => {
                    file.season = 1;
                });
            return;
        }
        files
            .filter(file => file.episodes && !file.isMovie && file.season !== 0)
            .filter(file => !this.isNewEpisodeNotInMetadata(torrent, file, metadata))
            .filter(file => !file.season || (metadata.episodeCount[file.season - 1] || 0) < file.episodes[0])
            .forEach(file => {
                const seasonIdx = ([...metadata.episodeCount.keys()]
                        .find((i) => metadata.episodeCount.slice(0, i + 1).reduce((a, b) => a + b) >= file.episodes[0])
                    + 1 || metadata.episodeCount.length) - 1;

                file.season = seasonIdx + 1;
                file.episodes = file.episodes
                    .map(ep => ep - metadata.episodeCount.slice(0, seasonIdx).reduce((a, b) => a + b, 0))
            });
    }

    private decomposeDateEpisodeFiles(files: ParsableTorrentFile[], metadata: MetadataResponse) {
        if (!metadata || !metadata.videos || !metadata.videos.length) {
            return;
        }

        const timeZoneOffset = this.getTimeZoneOffset(metadata.country);
        const offsetVideos = metadata.videos
            .reduce((map, video) => {
                const releaseDate = moment(video.released).utcOffset(timeZoneOffset).format('YYYY-MM-DD');
                map[releaseDate] = video;
                return map;
            }, {});

        files
            .filter(file => file.date)
            .forEach(file => {
                const video = offsetVideos[file.date];
                if (video) {
                    file.season = video.season;
                    file.episodes = [video.episode];
                }
            });
    }

    private getTimeZoneOffset(country: string | undefined) {
        switch (country) {
            case 'United States':
            case 'USA':
                return '-08:00';
            default:
                return '00:00';
        }
    }

    private assignKitsuOrImdbEpisodes(torrent: TorrentInfo, files: ParsableTorrentFile[], metadata: MetadataResponse) {
        if (!metadata || !metadata.videos || !metadata.videos.length) {
            if (torrent.type === TorrentType.Anime) {
                // assign episodes as kitsu episodes for anime when no metadata available for imdb mapping
                files
                    .filter(file => file.season && file.episodes)
                    .forEach(file => {
                        file.season = undefined;
                        file.episodes = undefined;
                    })
                if (metadata.type === TorrentType.Movie && files.every(file => !file.imdbId)) {
                    // sometimes a movie has episode naming, thus not recognized as a movie and imdbId not assigned
                    files.forEach(file => file.imdbId = metadata.imdbId);
                }
            }
            return files;
        }

        const seriesMapping: CommonVideoMetadata = metadata.videos
            .reduce((map, video) => {
                const episodeMap = map[video.season] || {};
                episodeMap[video.episode] = video;
                map[video.season] = episodeMap;
                return map;
            }, {});

        if (metadata.videos.some(video => Number.isInteger(video.season)) || !metadata.imdbId) {
            files.filter((file => Number.isInteger(file.season) && file.episodes))
                .map(file => {
                    const seasonMapping = seriesMapping[file.season];
                    const episodeMapping = seasonMapping && seasonMapping[file.episodes[0]];
                    if (episodeMapping && Number.isInteger(episodeMapping.imdbSeason)) {
                        file.imdbId = metadata.imdbId;
                        file.season = episodeMapping.imdbSeason;
                        file.episodes = file.episodes.map(ep => seasonMapping[ep] && seasonMapping[ep].imdbEpisode);
                    } else {
                        // no imdb mapping available for episode
                        file.season = undefined;
                        file.episodes = undefined;
                    }
                });
        } else if (metadata.videos.some(video => video.episode)) {
            // imdb episode info is base
            files
                .filter(file => Number.isInteger(file.season) && file.episodes)
                .forEach(file => {
                    if (seriesMapping[file.season]) {
                        const seasonMapping = seriesMapping[file.season];
                        file.imdbId = metadata.imdbId;
                        file.kitsuId = seasonMapping[file.episodes[0]] && seasonMapping[file.episodes[0]].kitsuId;
                        file.episodes = file.episodes.map(ep => seasonMapping[ep] && seasonMapping[ep].kitsuEpisode);
                    } else if (seriesMapping[file.season - 1]) {
                        // sometimes a second season might be a continuation of the previous season
                        const seasonMapping = seriesMapping[file.season - 1] as CommonVideoMetadata;
                        const episodes = Object.values(seasonMapping);
                        const firstKitsuId = episodes.length && episodes[0];
                        const differentTitlesCount = new Set(episodes.map(ep => ep.id)).size
                        const skippedCount = episodes.filter(ep => ep.id === firstKitsuId).length;
                        const seasonEpisodes = files
                            .filter(otherFile => otherFile.season === file.season)
                            .reduce((a, b) => a.episodes.concat(b.episodes), []);
                        const isAbsoluteOrder = seasonEpisodes.episodes.every(ep => ep > skippedCount && ep <= episodes.length)
                        const isNormalOrder = seasonEpisodes.episodes.every(ep => ep + skippedCount <= episodes.length)
                        if (differentTitlesCount >= 1 && (isAbsoluteOrder || isNormalOrder)) {
                            file.imdbId = metadata.imdbId;
                            file.season = file.season - 1;
                            file.episodes = file.episodes.map(ep => isAbsoluteOrder ? ep : ep + skippedCount);
                            file.kitsuId = seasonMapping[file.episodes[0]].kitsuId;
                            file.episodes = file.episodes.map(ep => seasonMapping[ep] && seasonMapping[ep].kitsuEpisode);
                        }
                    } else if (Object.values(seriesMapping).length === 1 && seriesMapping[1]) {
                        // sometimes series might be named with sequel season but it's not a season on imdb and a new title
                        const seasonMapping = seriesMapping[1];
                        file.imdbId = metadata.imdbId;
                        file.season = 1;
                        file.kitsuId = seasonMapping[file.episodes[0]].kitsuId;
                        file.episodes = file.episodes.map(ep => seasonMapping[ep] && seasonMapping[ep].kitsuEpisode);
                    }
                });
        }
        return files;
    }

    private needsCinemetaMetadataForAnime(files: ParsableTorrentFile[], metadata: MetadataResponse) {
        if (!metadata || !metadata.imdbId || !metadata.videos || !metadata.videos.length) {
            return false;
        }

        const minSeason = Math.min(...metadata.videos.map(video => video.season)) || Number.MAX_VALUE;
        const maxSeason = Math.max(...metadata.videos.map(video => video.season)) || Number.MAX_VALUE;
        const differentSeasons = new Set(metadata.videos
            .map(video => video.season)
            .filter(season => Number.isInteger(season))).size;
        const total = metadata.totalCount || Number.MAX_VALUE;
        return differentSeasons > 1 || files
            .filter(file => !file.isMovie && file.episodes)
            .some(file => file.season < minSeason || file.season > maxSeason || file.episodes.every(ep => ep > total));
    }

    private async updateToCinemetaMetadata(metadata: MetadataResponse) {
        const query: MetaDataQuery = {
            id: metadata.imdbId,
            type: metadata.type
        };

        return await metadataService.getMetadata(query)
            .then((newMetadataOrError) => {
                if (newMetadataOrError instanceof Error) {
                    // handle error
                    logger.warn(`Failed ${metadata.imdbId} metadata cinemeta update due: ${newMetadataOrError.message}`);
                    return metadata;    // or throw newMetadataOrError to propagate error up the call stack
                }
                // At this point TypeScript infers newMetadataOrError to be of type MetadataResponse
                let newMetadata = newMetadataOrError;
                if (!newMetadata.videos || !newMetadata.videos.length) {
                    return metadata;
                } else {
                    metadata.videos = newMetadata.videos;
                    metadata.episodeCount = newMetadata.episodeCount;
                    metadata.totalCount = newMetadata.totalCount;
                    return metadata;
                }
            })
    }

    private findMovieImdbId(title: ParseTorrentTitleResult | string) {
        const parsedTitle = typeof title === 'string' ? parse(title) : title;
        logger.debug(`Finding movie imdbId for ${title}`);
        return this.imdb_limiter.schedule(async () => {
            const imdbQuery = {
                title: parsedTitle.title,
                year: parsedTitle.year,
                type: TorrentType.Movie
            };
            try {
                return await metadataService.getImdbId(imdbQuery);
            } catch (e) {
                return undefined;
            }
        });
    }

    private async findMovieKitsuId(title: ParseTorrentTitleResult | string) {
        const parsedTitle = typeof title === 'string' ? parse(title) : title;
        const kitsuQuery = {
            title: parsedTitle.title,
            year: parsedTitle.year,
            season: parsedTitle.season,
            type: TorrentType.Movie
        };
        try {
            return await metadataService.getKitsuId(kitsuQuery);
        } catch (e) {
            return undefined;
        }
    }

    private isDiskTorrent(contents: ParsableTorrentFile[]) {
        return contents.some(content => extensionService.isDisk(content.path));
    }

    private isSingleMovie(videos: ParsableTorrentFile[]) {
        return videos.length === 1 ||
            (videos.length === 2 &&
                videos.find(v => /\b(?:part|disc|cd)[ ._-]?0?1\b|^0?1\.\w{2,4}$/i.test(v.path)) &&
                videos.find(v => /\b(?:part|disc|cd)[ ._-]?0?2\b|^0?2\.\w{2,4}$/i.test(v.path)));
    }

    private isFeaturette(video: ParsableTorrentFile) {
        return /featurettes?\/|extras-grym/i.test(video.path);
    }

    private clearInfoFields(video: ParsableTorrentFile) {
        video.imdbId = undefined;
        video.season = undefined;
        video.episode = undefined;
        video.kitsuId = undefined;
        return video;
    }

    private div100(episode: number) {
        return (episode / 100 >> 0); // floor to nearest int
    }

    private mod100(episode: number) {
        return episode % 100;
    }
}

export const torrentFileService = new TorrentFileService();






















