import {TorrentType} from '@enums/torrent_types';
import {ExtensionHelpers} from '@helpers/extension_helpers';
import {PromiseHelpers} from '@helpers/promises_helpers';
import {ICommonVideoMetadata} from "@interfaces/common_video_metadata";
import {ILoggingService} from "@interfaces/logging_service";
import {IMetaDataQuery} from "@interfaces/metadata_query";
import {IMetadataResponse} from "@interfaces/metadata_response";
import {IMetadataService} from "@interfaces/metadata_service";
import {IParsedTorrent} from "@interfaces/parsed_torrent";
import {ISeasonEpisodeMap} from "@interfaces/season_episode_map";
import {ITorrentDownloadService} from "@interfaces/torrent_download_service";
import {ITorrentFileCollection} from "@interfaces/torrent_file_collection";
import {ITorrentFileService} from "@interfaces/torrent_file_service";
import {IocTypes} from "@models/ioc_types";
import {IContentAttributes} from "@repository/interfaces/content_attributes";
import {IFileAttributes} from "@repository/interfaces/file_attributes";
import {configurationService} from '@services/configuration_service';
import Bottleneck from 'bottleneck';
import {inject, injectable} from "inversify";
import moment from 'moment';
import {parse} from 'parse-torrent-title';

const MIN_SIZE: number = 5 * 1024 * 1024; // 5 MB
const MULTIPLE_FILES_SIZE = 4 * 1024 * 1024 * 1024; // 4 GB

@injectable()
export class TorrentFileService implements ITorrentFileService {
    private metadataService: IMetadataService;
    private torrentDownloadService: ITorrentDownloadService;
    private logger: ILoggingService;
    private readonly imdb_limiter: Bottleneck = new Bottleneck({
        maxConcurrent: configurationService.metadataConfig.IMDB_CONCURRENT,
        minTime: configurationService.metadataConfig.IMDB_INTERVAL_MS
    });

    constructor(@inject(IocTypes.IMetadataService) metadataService: IMetadataService,
                @inject(IocTypes.ITorrentDownloadService) torrentDownloadService: ITorrentDownloadService,
                @inject(IocTypes.ILoggingService) logger: ILoggingService) {
        this.metadataService = metadataService;
        this.torrentDownloadService = torrentDownloadService;
        this.logger = logger;
    }

    public parseTorrentFiles = async (torrent: IParsedTorrent): Promise<ITorrentFileCollection> => {
        if (!torrent.title) {
            return Promise.reject(new Error('Torrent title is missing'));
        }

        if (!torrent.infoHash) {
            return Promise.reject(new Error('Torrent infoHash is missing'));
        }

        const parsedTorrentName = parse(torrent.title);
        const query: IMetaDataQuery = {
            id: torrent.kitsuId || torrent.imdbId,
            type: torrent.type || TorrentType.Movie,
        };
        const metadata = await this.metadataService.getMetadata(query)
            .then(meta => Object.assign({}, meta))
            .catch(() => undefined);

        if (metadata === undefined || metadata instanceof Error) {
            return Promise.reject(new Error('Failed to retrieve metadata'));
        }

        if (torrent.type !== TorrentType.Anime && metadata && metadata.type && metadata.type !== torrent.type) {
            // it's actually a movie/series
            torrent.type = metadata.type;
        }

        if (torrent.type === TorrentType.Movie && (!parsedTorrentName.seasons ||
            parsedTorrentName.season === 5 && [1, 5].includes(parsedTorrentName.episode || 0))) {
            return this.parseMovieFiles(torrent, metadata);
        }

        return this.parseSeriesFiles(torrent, metadata)
    };

    public isPackTorrent = (torrent: IParsedTorrent): boolean => {
        if (torrent.isPack) {
            return true;
        }
        if (!torrent.title) {
            return false;
        }
        const parsedInfo = parse(torrent.title);
        if (torrent.type === TorrentType.Movie) {
            return parsedInfo.complete || typeof parsedInfo.year === 'string' || /movies/i.test(torrent.title);
        }

        const hasMultipleEpisodes = Boolean(parsedInfo.complete || torrent.size || 0 > MULTIPLE_FILES_SIZE ||
            (parsedInfo.seasons && parsedInfo.seasons.length > 1) ||
            (parsedInfo.episodes && parsedInfo.episodes.length > 1) ||
            (parsedInfo.seasons && !parsedInfo.episodes));

        const hasSingleEpisode: boolean = Boolean(Number.isInteger(parsedInfo.episode) || (!parsedInfo.episodes && parsedInfo.date));

        return hasMultipleEpisodes && !hasSingleEpisode;
    };

    private parseSeriesVideos = (torrent: IParsedTorrent, videos: IFileAttributes[]): IFileAttributes[] => {
        const parsedTorrentName = parse(torrent.title!);
        const hasMovies = parsedTorrentName.complete || !!torrent.title!.match(/movies?(?:\W|$)/i);
        const parsedVideos = videos.map(video => this.parseSeriesVideo(video));

        return parsedVideos.map(video => ({...video, isMovie: this.isMovieVideo(torrent, video, parsedVideos, hasMovies)}));
    };

    private parseMovieFiles = async (torrent: IParsedTorrent, metadata: IMetadataResponse): Promise<ITorrentFileCollection> => {
        const fileCollection: ITorrentFileCollection = await this.getMoviesTorrentContent(torrent);
        if (fileCollection.videos === undefined || fileCollection.videos.length === 0) {
            return {...fileCollection, videos: this.getDefaultFileEntries(torrent)};
        }

        const filteredVideos = fileCollection.videos
            .filter(video => video.size! > MIN_SIZE)
            .filter(video => !this.isFeaturette(video));
        if (this.isSingleMovie(filteredVideos)) {
            const parsedVideos = filteredVideos.map(video => ({
                infoHash: torrent.infoHash,
                fileIndex: video.fileIndex,
                title: video.title || video.path || video.fileName || '',
                size: video.size || torrent.size,
                imdbId: torrent.imdbId?.toString() || metadata && metadata.imdbId?.toString(),
                kitsuId: parseInt(torrent.kitsuId?.toString() || metadata && metadata.kitsuId?.toString() || '0')
            }));
            return {...fileCollection, videos: parsedVideos};
        }

        const parsedVideos = await PromiseHelpers.sequence(filteredVideos.map(video => () => this.isFeaturette(video)
            ? Promise.resolve(video)
            : this.findMovieImdbId(video.title).then(imdbId => ({...video, imdbId: imdbId?.toString() || ''}))))
            .then(videos => videos.map((video: IFileAttributes) => ({
                infoHash: torrent.infoHash,
                fileIndex: video.fileIndex,
                title: video.title || video.path,
                size: video.size,
                imdbId: video.imdbId,
            })));
        return {...fileCollection, videos: parsedVideos};
    };

    private parseSeriesFiles = async (torrent: IParsedTorrent, metadata: IMetadataResponse): Promise<ITorrentFileCollection> => {
        const fileCollection: ITorrentFileCollection = await this.getSeriesTorrentContent(torrent);
        if (fileCollection.videos === undefined || fileCollection.videos.length === 0) {
            return {...fileCollection, videos: this.getDefaultFileEntries(torrent)};
        }

        const parsedVideos: IFileAttributes[] = await Promise.resolve(fileCollection.videos)
            .then(videos => videos.filter(video => videos?.length === 1 || video.size! > MIN_SIZE))
            .then(videos => this.parseSeriesVideos(torrent, videos))
            .then(videos => this.decomposeEpisodes(torrent, videos, metadata))
            .then(videos => this.assignKitsuOrImdbEpisodes(torrent, videos, metadata))
            .then(videos => Promise.all(videos.map(video => video.isMovie
                ? this.mapSeriesMovie(torrent, video)
                : this.mapSeriesEpisode(torrent, video, videos))))
            .then(videos => videos
                .reduce((a, b) => a.concat(b), [])
                .map(video => this.isFeaturette(video) ? this.clearInfoFields(video) : video));
        return {...torrent.fileCollection, videos: parsedVideos};
    };

    private getMoviesTorrentContent = async (torrent: IParsedTorrent): Promise<ITorrentFileCollection> => {
        const files = await this.torrentDownloadService.getTorrentFiles(torrent, configurationService.torrentConfig.TIMEOUT)
            .catch(error => {
                if (!this.isPackTorrent(torrent)) {
                    const entries = this.getDefaultFileEntries(torrent);
                    return {videos: entries, contents: [], subtitles: [], files: entries}
                }
                return Promise.reject(error);
            });

        if (files.contents && files.contents.length && !files.videos?.length && this.isDiskTorrent(files.contents)) {
            files.videos = this.getDefaultFileEntries(torrent);
        }

        return files;
    };

    private getDefaultFileEntries = (torrent: IParsedTorrent): IFileAttributes[] => [{
        title: torrent.title!,
        path: torrent.title,
        size: torrent.size,
        fileIndex: 0,
    }];

    private getSeriesTorrentContent = async (torrent: IParsedTorrent): Promise<ITorrentFileCollection> => this.torrentDownloadService.getTorrentFiles(torrent, configurationService.torrentConfig.TIMEOUT)
        .catch(error => {
            if (!this.isPackTorrent(torrent)) {
                return {videos: this.getDefaultFileEntries(torrent), subtitles: [], contents: []}
            }
            return Promise.reject(error);
        });

    private mapSeriesEpisode = async (torrent: IParsedTorrent, file: IFileAttributes, files: IFileAttributes[]): Promise<IFileAttributes[]> => {
        if (!file.episodes && !file.episodes) {
            if (files.length === 1 || files.some(f => f.episodes || f.episodes) || parse(torrent.title!).seasons) {
                return Promise.resolve([{
                    infoHash: torrent.infoHash,
                    fileIndex: file.fileIndex,
                    title: file.path || file.title,
                    size: file.size,
                    imdbId: torrent?.imdbId?.toString() || file?.imdbId?.toString() || '',
                }]);
            }
            return Promise.resolve([]);
        }
        const episodeIndexes = [...(file.episodes || file.episodes).keys()];
        return Promise.resolve(episodeIndexes.map((index) => ({
            infoHash: torrent.infoHash,
            fileIndex: file.fileIndex,
            title: file.path || file.title,
            size: file.size,
            imdbId: file?.imdbId?.toString() || torrent?.imdbId?.toString() || '',
            imdbSeason: file.season,
            season: file.season,
            imdbEpisode: file.episodes && file.episodes[index],
            episode: file.episodes && file.episodes[index],
            kitsuEpisode: file.episodes && file.episodes[index],
            episodes: file.episodes,
            kitsuId: parseInt(file.kitsuId?.toString() || torrent.kitsuId?.toString() || '0') || 0,
        })))
    };

    private mapSeriesMovie = async (torrent: IParsedTorrent, file: IFileAttributes): Promise<IFileAttributes[]> => {
        const kitsuId = torrent.type === TorrentType.Anime ? await this.findMovieKitsuId(file)
            .then(result => {
                if (result instanceof Error) {
                    this.logger.warn(`Failed to retrieve kitsuId due to error: ${result.message}`);
                    return undefined;
                }
                return result;
            }) : undefined;

        const imdbId = !kitsuId ? await this.findMovieImdbId(file) : undefined;

        const query: IMetaDataQuery = {
            id: kitsuId || imdbId,
            type: TorrentType.Movie
        };

        const metadataOrError = await this.metadataService.getMetadata(query);
        if (metadataOrError instanceof Error) {
            this.logger.warn(`Failed to retrieve metadata due to error: ${metadataOrError.message}`);
            // return default result or throw error, depending on your use case
            return [{
                infoHash: torrent.infoHash,
                fileIndex: file.fileIndex,
                title: file.path || file.title,
                size: file.size,
                imdbId: imdbId,
                kitsuId: parseInt(kitsuId?.toString() || '0') || 0,
                episodes: undefined,
                imdbSeason: undefined,
                imdbEpisode: undefined,
                kitsuEpisode: undefined
            }];
        }
        // at this point, TypeScript infers that metadataOrError is actually MetadataResponse
        const metadata = metadataOrError;
        const hasEpisode = metadata.videos && metadata.videos.length && (file.episode || metadata.videos.length === 1);
        const episodeVideo = hasEpisode && metadata.videos && metadata.videos[(file.episode || 1) - 1];
        return [{
            infoHash: torrent.infoHash,
            fileIndex: file.fileIndex,
            title: file.path || file.title,
            size: file.size,
            imdbId: metadata.imdbId?.toString() || imdbId || '',
            kitsuId: parseInt(metadata.kitsuId?.toString() || kitsuId?.toString() || '0') || 0,
            imdbSeason: episodeVideo && metadata.imdbId ? episodeVideo.season : undefined,
            imdbEpisode: episodeVideo && metadata.imdbId || episodeVideo && metadata.kitsuId ? episodeVideo.episode || episodeVideo.episode : undefined,
            kitsuEpisode: episodeVideo && metadata.imdbId || episodeVideo && metadata.kitsuId ? episodeVideo.episode || episodeVideo.episode : undefined,
        }];
    };

    private decomposeEpisodes = async (torrent: IParsedTorrent, files: IFileAttributes[], metadata: IMetadataResponse = {episodeCount: []}): Promise<IFileAttributes[]> => {
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
    };

    private preprocessEpisodes = (files: IFileAttributes[]): void => {
        // reverse special episode naming when they named with 0 episode, ie. S02E00
        files
            .filter(file => Number.isInteger(file.season) && file.episode === 0)
            .forEach(file => {
                file.episode = file.season
                file.episodes = [file.season || 0];
                file.season = 0;
            })
    };

    private isConcatSeasonAndEpisodeFiles = (files: IFileAttributes[], sortedEpisodes: number[], metadata: IMetadataResponse): boolean => {
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
            .filter(ep => metadata.episodeCount && metadata.episodeCount[this.div100(ep) - 1] < ep)
            .filter(ep => metadata.episodeCount && metadata.episodeCount[this.div100(ep) - 1] >= this.mod100(ep));
        const concatFileEpisodes = files
            .filter(file => !file.isMovie && file.episodes)
            .filter(file => !file.season || file.episodes?.every(ep => this.div100(ep) === file.season));
        const concatAboveTotalEpisodeCount = files
            .filter(file => !file.isMovie && file.episodes && file.episodes.every(ep => ep > 100))
            .filter(file => file.episodes?.every(ep => ep > metadata.totalCount!));
        return sortedConcatEpisodes.length >= thresholdSorted && concatFileEpisodes.length >= threshold
            || concatAboveTotalEpisodeCount.length >= thresholdAbove;
    };

    private isDateEpisodeFiles = (files: IFileAttributes[], metadata: IMetadataResponse): boolean => files.every(file => (!file.season || metadata.episodeCount && !metadata.episodeCount[file.season - 1]) && file.date);

    private isAbsoluteEpisodeFiles = (torrent: IParsedTorrent, files: IFileAttributes[], metadata: IMetadataResponse): boolean => {
        const threshold = Math.ceil(files.length / 5);
        const isAnime = torrent.type === TorrentType.Anime && torrent.kitsuId;
        const nonMovieEpisodes = files.filter(file => !file.isMovie && file.episodes);
        const absoluteEpisodes = files
            .filter(file => file.season && file.episodes)
            .filter(file => file.episodes?.every(ep =>
                metadata.episodeCount && file.season && metadata.episodeCount[file.season - 1] < ep));
        return nonMovieEpisodes.every(file => !file.season)
            || (isAnime && nonMovieEpisodes.every(file =>
                metadata.episodeCount && file.season && file.season > metadata.episodeCount.length))
            || absoluteEpisodes.length >= threshold;
    };

    private isNewEpisodeNotInMetadata = (torrent: IParsedTorrent, video: IFileAttributes, metadata: IMetadataResponse): boolean => {
        const isAnime = torrent.type === TorrentType.Anime && torrent.kitsuId;
        return !!(!isAnime && !video.isMovie && video.episodes && video.season !== 1
            && metadata.status && /continuing|current/i.test(metadata.status)
            && metadata.episodeCount && video.season && video.season >= metadata.episodeCount.length
            && video.episodes.every(ep => metadata.episodeCount && video.season && ep > (metadata.episodeCount[video.season - 1] || 0)));
    };

    private decomposeConcatSeasonAndEpisodeFiles = (files: IFileAttributes[], metadata: IMetadataResponse): void => {
        files
            .filter(file => file.episodes && file.season !== 0 && file.episodes.every(ep => ep > 100))
            .filter(file => file.episodes && metadata?.episodeCount &&
                ((file.season || this.div100(file.episodes[0])) - 1) >= 0 &&
                metadata.episodeCount[(file.season || this.div100(file.episodes[0])) - 1] < 100)
            .filter(file => (file.season && file.episodes && file.episodes.every(ep => this.div100(ep) === file.season)) || !file.season)
            .forEach(file => {
                if (file.episodes) {
                    file.season = this.div100(file.episodes[0]);
                    file.episodes = file.episodes.map(ep => this.mod100(ep));
                }
            });
    };

    private decomposeAbsoluteEpisodeFiles = (torrent: IParsedTorrent, videos: IFileAttributes[], metadata: IMetadataResponse): void => {
        if (metadata.episodeCount?.length === 0) {
            videos
                .filter(file => !Number.isInteger(file.season) && file.episodes && !file.isMovie)
                .forEach(file => {
                    file.season = 1;
                });
            return;
        }
        if (!metadata.episodeCount) return;

        videos
            .filter(file => file.episodes && !file.isMovie && file.season !== 0)
            .filter(file => !this.isNewEpisodeNotInMetadata(torrent, file, metadata))
            .filter(file => {
                if (!file.episodes || !metadata.episodeCount) return false;
                return !file.season || (metadata.episodeCount[file.season - 1] || 0) < file.episodes[0];
            })
            .forEach(file => {
                if (!file.episodes || !metadata.episodeCount) return;

                let seasonIdx = metadata.episodeCount
                    .map((_, i) => i)
                    .find(i => metadata.episodeCount && file.episodes && metadata.episodeCount.slice(0, i + 1).reduce((a, b) => a + b) >= file.episodes[0]);

                seasonIdx = (seasonIdx || 1 || metadata.episodeCount.length) - 1;

                file.season = seasonIdx + 1;
                file.episodes = file.episodes
                    .map(ep => ep - (metadata.episodeCount?.slice(0, seasonIdx).reduce((a, b) => a + b, 0) || 0));
            });
    };

    private decomposeDateEpisodeFiles = (files: IFileAttributes[], metadata: IMetadataResponse): void => {
        if (!metadata || !metadata.videos || !metadata.videos.length) {
            return;
        }

        const timeZoneOffset = this.getTimeZoneOffset(metadata.country);
        const offsetVideos: { [key: string]: ICommonVideoMetadata } = metadata.videos
            .reduce((map: { [key: string]: ICommonVideoMetadata }, video: ICommonVideoMetadata) => {
                const releaseDate = moment(video.released).utcOffset(timeZoneOffset).format('YYYY-MM-DD');
                map[releaseDate] = video;
                return map;
            }, {});

        files
            .filter(file => file.date)
            .forEach(file => {
                const video = offsetVideos[file.date!];
                if (video) {
                    file.season = video.season;
                    file.episodes = [video.episode || 0];
                }
            });
    };

    private getTimeZoneOffset = (country: string | undefined): string => {
        switch (country) {
            case 'United States':
            case 'USA':
                return '-08:00';
            default:
                return '00:00';
        }
    };

    private assignKitsuOrImdbEpisodes = (torrent: IParsedTorrent, files: IFileAttributes[], metadata: IMetadataResponse): IFileAttributes[] => {
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
                    files.forEach(file => file.imdbId = metadata.imdbId?.toString());
                }
            }
            return files;
        }

        const seriesMapping = metadata.videos
            .filter(video => video.season !== undefined && Number.isInteger(video.season) && video.episode !== undefined && Number.isInteger(video.episode))
            .reduce<ISeasonEpisodeMap>((map, video) => {
                if (video.season !== undefined && video.episode !== undefined) {
                    const episodeMap = map[video.season] || {};
                    episodeMap[video.episode] = video;
                    map[video.season] = episodeMap;
                }
                return map;
            }, {});


        if (metadata.videos.some(video => Number.isInteger(video.season)) || !metadata.imdbId) {
            files.filter(file => file && Number.isInteger(file.season) && file.episodes)
                .map(file => {
                    const seasonMapping = file && file.season && seriesMapping[file.season] || null;
                    const episodeMapping = seasonMapping && file && file.episodes && file.episodes[0] && seasonMapping[file.episodes[0]] || null;

                    if (episodeMapping && Number.isInteger(episodeMapping.season)) {
                        file.imdbId = metadata.imdbId?.toString();
                        file.season = episodeMapping.season;
                        file.episodes = file.episodes && file.episodes.map(ep => (seasonMapping && seasonMapping[ep]) ? Number(seasonMapping[ep].episode) : 0);
                    } else {
                        file.season = undefined;
                        file.episodes = undefined;
                    }
                });
        } else if (metadata.videos.some(video => video.episode)) {
            // imdb episode info is base
            files
                .filter(file => Number.isInteger(file.season) && file.episodes)
                .forEach(file => {
                    if (!file.season || !file.episodes) {
                        return;
                    }
                    if (seriesMapping[file.season]) {

                        const seasonMapping = seriesMapping[file.season];
                        file.imdbId = metadata.imdbId?.toString();
                        file.kitsuId = seasonMapping[file.episodes[0]] && parseInt(seasonMapping[file.episodes[0]].id || '0') || 0;
                        file.episodes = file.episodes.map(ep => seasonMapping[ep]?.episode)
                            .filter((ep): ep is number => ep !== undefined);
                    } else if (seriesMapping[file.season - 1]) {
                        // sometimes a second season might be a continuation of the previous season
                        const seasonMapping = seriesMapping[file.season - 1] as ICommonVideoMetadata;
                        const episodes = Object.values(seasonMapping);
                        const firstKitsuId = episodes.length && episodes[0];
                        const differentTitlesCount = new Set(episodes.map(ep => ep.id)).size
                        const skippedCount = episodes.filter(ep => ep.id === firstKitsuId).length;
                        const emptyArray: number[] = [];
                        const seasonEpisodes = files
                            .filter((otherFile: IFileAttributes) => otherFile.season === file.season && otherFile.episodes)
                            .reduce((a, b) => a.concat(b.episodes || []), emptyArray);
                        const isAbsoluteOrder = seasonEpisodes.every(ep => ep > skippedCount && ep <= episodes.length)
                        const isNormalOrder = seasonEpisodes.every(ep => ep + skippedCount <= episodes.length)
                        if (differentTitlesCount >= 1 && (isAbsoluteOrder || isNormalOrder)) {
                            const {season} = file;
                            const [episode] = file.episodes;
                            file.imdbId = metadata.imdbId?.toString();
                            file.season = file.season - 1;
                            file.episodes = file.episodes.map(ep => isAbsoluteOrder ? ep : ep + skippedCount);
                            const currentEpisode = seriesMapping[season][episode];
                            file.kitsuId = currentEpisode ? parseInt(currentEpisode.id || '0') : 0;
                            if (typeof season === 'number' && Array.isArray(file.episodes)) {
                                file.episodes = file.episodes.map(ep =>
                                    seriesMapping[season]
                                    && seriesMapping[season][ep]
                                    && seriesMapping[season][ep].episode
                                    || ep);
                            }
                        }
                    } else if (Object.values(seriesMapping).length === 1 && seriesMapping[1]) {
                        // sometimes series might be named with sequel season but it's not a season on imdb and a new title
                        // eslint-disable-next-line prefer-destructuring
                        const seasonMapping = seriesMapping[1];
                        file.imdbId = metadata.imdbId?.toString();
                        file.season = 1;
                        file.kitsuId = parseInt(seasonMapping[file.episodes[0]].id || '0') || 0;
                        file.episodes = file.episodes.map(ep => seasonMapping[ep] && seasonMapping[ep].episode)
                            .filter((ep): ep is number => ep !== undefined);
                    }
                });
        }
        return files;
    };

    private needsCinemetaMetadataForAnime = (files: IFileAttributes[], metadata: IMetadataResponse): boolean => {
        if (!metadata || !metadata.imdbId || !metadata.videos || !metadata.videos.length) {
            return false;
        }

        const seasons = metadata.videos
            .map(video => video.season)
            .filter((season): season is number => season !== null && season !== undefined);

        // Using || 0 instead of || Number.MAX_VALUE to match previous logic
        const minSeason = Math.min(...seasons) || 0;
        const maxSeason = Math.max(...seasons) || 0;
        const differentSeasons = new Set(seasons.filter(season => Number.isInteger(season))).size;

        const total = metadata.totalCount || Number.MAX_VALUE;

        return differentSeasons > 1 || files
            .filter(file => !file.isMovie && file.episodes)
            .some(file => file.season || 0 < minSeason || file.season || 0 > maxSeason || file.episodes?.every(ep => ep > total));
    };

    private updateToCinemetaMetadata = async (metadata: IMetadataResponse): Promise<IMetadataResponse> => {
        const query: IMetaDataQuery = {
            id: metadata.imdbId,
            type: metadata.type
        };

        return await this.metadataService.getMetadata(query)
            .then((newMetadataOrError) => {
                if (newMetadataOrError instanceof Error) {
                    // handle error
                    this.logger.warn(`Failed ${metadata.imdbId} metadata cinemeta update due: ${newMetadataOrError.message}`);
                    return metadata;    // or throw newMetadataOrError to propagate error up the call stack
                }
                // At this point TypeScript infers newMetadataOrError to be of type MetadataResponse
                const newMetadata = newMetadataOrError;
                if (!newMetadata.videos || !newMetadata.videos.length) {
                    return metadata;
                } else {
                    metadata.videos = newMetadata.videos;
                    metadata.episodeCount = newMetadata.episodeCount;
                    metadata.totalCount = newMetadata.totalCount;
                    return metadata;
                }
            })
    };

    private findMovieImdbId = (title: IFileAttributes | string): Promise<string | undefined> => {
        const parsedTitle = typeof title === 'string' ? parse(title) : title;
        this.logger.debug(`Finding movie imdbId for ${title}`);
        return this.imdb_limiter.schedule(async () => {
            const imdbQuery = {
                title: parsedTitle.title,
                year: parsedTitle.year,
                type: TorrentType.Movie
            };
            try {
                return await this.metadataService.getImdbId(imdbQuery);
            } catch (e) {
                return undefined;
            }
        });
    };

    private findMovieKitsuId = async (title: IFileAttributes | string): Promise<number | Error | undefined> => {
        const parsedTitle = typeof title === 'string' ? parse(title) : title;
        const kitsuQuery = {
            title: parsedTitle.title,
            year: parsedTitle.year,
            season: parsedTitle.season,
            type: TorrentType.Movie
        };
        try {
            return await this.metadataService.getKitsuId(kitsuQuery);
        } catch (e) {
            return undefined;
        }
    };

    private isDiskTorrent = (contents: IContentAttributes[]): boolean => contents.some(content => ExtensionHelpers.isDisk(content.path));

    private isSingleMovie = (videos: IFileAttributes[]): boolean => videos.length === 1 ||
        (videos.length === 2 &&
            videos.find(v => /\b(?:part|disc|cd)[ ._-]?0?1\b|^0?1\.\w{2,4}$/i.test(v.path!)) &&
            videos.find(v => /\b(?:part|disc|cd)[ ._-]?0?2\b|^0?2\.\w{2,4}$/i.test(v.path!))) !== undefined;

    private isFeaturette = (video: IFileAttributes): boolean => /featurettes?\/|extras-grym/i.test(video.path!);

    private parseSeriesVideo = (video: IFileAttributes): IFileAttributes => {
        const videoInfo = parse(video.title);
        // the episode may be in a folder containing season number
        if (!Number.isInteger(videoInfo.season) && video.path?.includes('/')) {
            const folders = video.path?.split('/');
            const pathInfo = parse(folders[folders.length - 2]);
            videoInfo.season = pathInfo.season;
        }
        if (!Number.isInteger(videoInfo.season) && video.season) {
            videoInfo.season = video.season;
        }
        if (!Number.isInteger(videoInfo.season) && videoInfo.seasons && videoInfo.seasons.length > 1) {
            // in case single file was interpreted as having multiple seasons
            [videoInfo.season] = videoInfo.seasons;
        }
        if (!Number.isInteger(videoInfo.season) && video.path?.includes('/') && video.seasons
            && video.seasons.length > 1) {
            // russian season are usually named with 'series name-2` i.e. Улицы разбитых фонарей-6/22. Одиночный выстрел.mkv
            const folderPathSeasonMatch = video.path?.match(/[\u0400-\u04ff]-(\d{1,2})(?=.*\/)/);
            videoInfo.season = folderPathSeasonMatch && parseInt(folderPathSeasonMatch[1], 10) || undefined;
        }
        // sometimes video file does not have correct date format as in torrent title
        if (!videoInfo.episodes && !videoInfo.date && video.date) {
            videoInfo.date = video.date;
        }
        // limit number of episodes in case of incorrect parsing
        if (videoInfo.episodes && videoInfo.episodes.length > 20) {
            videoInfo.episodes = [videoInfo.episodes[0]];
            [videoInfo.episode] = videoInfo.episodes;
        }
        // force episode to any found number if it was not parsed
        if (!videoInfo.episodes && !videoInfo.date) {
            const epMatcher = videoInfo.title.match(
                /(?<!season\W*|disk\W*|movie\W*|film\W*)(?:^|\W|_)(\d{1,4})(?:a|b|c|v\d)?(?:_|\W|$)(?!disk|movie|film)/i);
            videoInfo.episodes = epMatcher && [parseInt(epMatcher[1], 10)] || undefined;
            videoInfo.episode = videoInfo.episodes && videoInfo.episodes[0];
        }
        if (!videoInfo.episodes && !videoInfo.date) {
            const epMatcher = video.title.match(new RegExp(`(?:\\(${videoInfo.year}\\)|part)[._ ]?(\\d{1,3})(?:\\b|_)`, "i"));
            videoInfo.episodes = epMatcher && [parseInt(epMatcher[1], 10)] || undefined;
            videoInfo.episode = videoInfo.episodes && videoInfo.episodes[0];
        }

        return {...video, ...videoInfo};
    };

    private isMovieVideo = (torrent: IParsedTorrent, video: IFileAttributes, otherVideos: IFileAttributes[], hasMovies: boolean): boolean => {
        if (Number.isInteger(torrent.season) && Array.isArray(torrent.episodes)) {
            // not movie if video has season
            return false;
        }
        if (torrent.title?.match(/\b(?:\d+[ .]movie|movie[ .]\d+)\b/i)) {
            // movie if video explicitly has numbered movie keyword in the name, ie. 1 Movie or Movie 1
            return true;
        }
        if (!hasMovies && torrent.type !== TorrentType.Anime) {
            // not movie if torrent name does not contain movies keyword or is not a pack torrent and is not anime
            return false;
        }
        if (!torrent.episodes) {
            // movie if there's no episode info it could be a movie
            return true;
        }
        // movie if contains year info and there aren't more than 3 video with same title and year
        // as some series titles might contain year in it.
        return !!torrent.year
            && otherVideos.length > 3
            && otherVideos.filter(other => other.title === video.title && other.year === video.year).length < 3;
    };

    private clearInfoFields = (video: IFileAttributes): IFileAttributes => {
        video.imdbId = undefined;
        video.imdbSeason = undefined;
        video.imdbEpisode = undefined;
        video.kitsuId = undefined;
        video.kitsuEpisode = undefined;
        return video;
    };

    private div100 = (episode: number): number => (episode / 100 >> 0);

    private mod100 = (episode: number): number => episode % 100;
}






















