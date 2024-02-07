import {inject, injectable} from "inversify";
import {parse} from 'parse-torrent-title';
import {TorrentType} from '../enums/torrent_types';
import {PromiseHelpers} from '../helpers/promises_helpers';
import {ILoggingService} from "../interfaces/logging_service";
import {IMetaDataQuery} from "../interfaces/metadata_query";
import {IMetadataService} from "../interfaces/metadata_service";
import {IParsedTorrent} from "../interfaces/parsed_torrent";
import {ITorrentEntriesService} from "../interfaces/torrent_entries_service";
import {ITorrentFileCollection} from "../interfaces/torrent_file_collection";
import {ITorrentFileService} from "../interfaces/torrent_file_service";
import {ITorrentSubtitleService} from "../interfaces/torrent_subtitle_service";
import {IocTypes} from "../models/ioc_types";
import {IDatabaseRepository} from "../repository/interfaces/database_repository";
import {IFileCreationAttributes} from "../repository/interfaces/file_attributes";
import {ISubtitleAttributes} from "../repository/interfaces/subtitle_attributes";
import {ITorrentAttributes, ITorrentCreationAttributes} from "../repository/interfaces/torrent_attributes";
import {File} from "../repository/models/file";
import {SkipTorrent} from "../repository/models/skipTorrent";
import {Subtitle} from "../repository/models/subtitle";
import {Torrent} from "../repository/models/torrent";

@injectable()
export class TorrentEntriesService implements ITorrentEntriesService {
    private metadataService: IMetadataService;
    private logger: ILoggingService;
    private fileService: ITorrentFileService;
    private subtitleService: ITorrentSubtitleService;
    private repository: IDatabaseRepository;

    constructor(@inject(IocTypes.IMetadataService) metadataService: IMetadataService,
                @inject(IocTypes.ILoggingService) logger: ILoggingService,
                @inject(IocTypes.ITorrentFileService) fileService: ITorrentFileService,
                @inject(IocTypes.ITorrentSubtitleService) torrentSubtitleService: ITorrentSubtitleService,
                @inject(IocTypes.IDatabaseRepository) repository: IDatabaseRepository) {
        this.metadataService = metadataService;
        this.logger = logger;
        this.fileService = fileService;
        this.subtitleService = torrentSubtitleService;
        this.repository = repository;
    }

    public createTorrentEntry = async (torrent: IParsedTorrent, overwrite = false): Promise<void> => {
        if (!torrent.title) {
            this.logger.warn(`No title found for ${torrent.provider} [${torrent.infoHash}]`);
            return;
        }
        
        const titleInfo = parse(torrent.title);

        if (!torrent.imdbId && torrent.type !== TorrentType.Anime) {
            const imdbQuery = {
                title: titleInfo.title,
                year: titleInfo.year,
                type: torrent.type
            };
            torrent.imdbId = await this.metadataService.getImdbId(imdbQuery)
                .catch(() => undefined);
        }
        if (torrent.imdbId && torrent.imdbId.toString().length < 9) {
            // pad zeros to imdbId if missing
            torrent.imdbId = 'tt' + torrent.imdbId.toString().replace('tt', '').padStart(7, '0');
        }
        if (torrent.imdbId && torrent.imdbId.toString().length > 9 && torrent.imdbId.toString().startsWith('tt0')) {
            // sanitize imdbId from redundant zeros
            torrent.imdbId = torrent.imdbId.toString().replace(/tt0+([0-9]{7,})$/, 'tt$1');
        }
        if (!torrent.kitsuId && torrent.type === TorrentType.Anime) {
            const kitsuQuery = {
                title: titleInfo.title,
                year: titleInfo.year,
                season: titleInfo.season,
            };

            await this.assignKitsuId(kitsuQuery, torrent);
        }

        if (!torrent.imdbId && !torrent.kitsuId && !this.fileService.isPackTorrent(torrent)) {
            this.logger.warn(`imdbId or kitsuId not found:  ${torrent.provider} ${torrent.title}`);
            return;
        }

        const fileCollection: ITorrentFileCollection = await this.fileService.parseTorrentFiles(torrent)
            .then((torrentContents: ITorrentFileCollection) => overwrite ? this.overwriteExistingFiles(torrent, torrentContents) : torrentContents)
            .then((torrentContents: ITorrentFileCollection) => this.subtitleService.assignSubtitles(torrentContents))
            .catch(error => {
                this.logger.warn(`Failed getting files for ${torrent.title}`, error.message);
                return {};
            });

        if (!fileCollection.videos || !fileCollection.videos.length) {
            this.logger.warn(`no video files found for ${torrent.provider} [${torrent.infoHash}] ${torrent.title}`);
            return;
        }

        const newTorrent: ITorrentCreationAttributes = ({
            ...torrent,
            contents: fileCollection.contents,
            subtitles: fileCollection.subtitles
        });
        
        return this.repository.createTorrent(newTorrent)
            .then(() => PromiseHelpers.sequence(fileCollection.videos!.map(video => () => {
                const newVideo: IFileCreationAttributes = {...video, infoHash: video.infoHash, title: video.title};
                if (!newVideo.kitsuId) {
                    newVideo.kitsuId = 0;
                }
                return this.repository.createFile(newVideo)
            })))
            .then(() => this.logger.info(`Created ${torrent.provider} entry for [${torrent.infoHash}] ${torrent.title}`));
    };

    private assignKitsuId = async (kitsuQuery: IMetaDataQuery, torrent: IParsedTorrent): Promise<void> => {
        await this.metadataService.getKitsuId(kitsuQuery)
            .then((result: number | Error) => {
                if (typeof result === 'number') {
                    torrent.kitsuId = result;
                } else {
                    torrent.kitsuId = 0;
                }
            })
            .catch((error: Error) => {
                this.logger.debug(`Failed getting kitsuId for ${torrent.title}`, error.message);
                torrent.kitsuId = 0;
            });
    };

    public createSkipTorrentEntry: (torrent: Torrent) => Promise<[SkipTorrent, boolean | null]> = async (torrent: Torrent)=> this.repository.createSkipTorrent(torrent.dataValues);

    public getStoredTorrentEntry = async (torrent: Torrent): Promise<Torrent | SkipTorrent | null | undefined> => this.repository.getSkipTorrent(torrent.infoHash)
        .catch(() => this.repository.getTorrent(torrent.dataValues))
        .catch(() => undefined);

    public checkAndUpdateTorrent = async (torrent: IParsedTorrent): Promise<boolean> => {
        const query: ITorrentAttributes = {
            infoHash: torrent.infoHash,
            provider: torrent.provider,
        }

        const existingTorrent = await this.repository.getTorrent(query).catch(() => undefined);

        if (!existingTorrent) {
            return false;
        }

        if (existingTorrent.provider === 'RARBG') {
            return true;
        }
        if (existingTorrent.provider === 'KickassTorrents' && torrent.provider) {
            existingTorrent.provider = torrent.provider;
            existingTorrent.torrentId = torrent.torrentId!;
        }

        if (!existingTorrent.languages && torrent.languages && existingTorrent.provider !== 'RARBG') {
            existingTorrent.languages = torrent.languages;
            await existingTorrent.save();
            this.logger.debug(`Updated [${existingTorrent.infoHash}] ${existingTorrent.title} language to ${torrent.languages}`);
        }
        
        return this.createTorrentContents(existingTorrent)
            .then(() => this.updateTorrentSeeders(existingTorrent.dataValues))
            .then(() => Promise.resolve(true))
            .catch(() => Promise.reject(false));
    };

    public createTorrentContents = async (torrent: Torrent): Promise<void> => {
        if (torrent.opened) {
            return;
        }

        const storedVideos: File[] = await this.repository.getFiles(torrent.infoHash).catch(() => []);
        if (!storedVideos || !storedVideos.length) {
            return;
        }
        const notOpenedVideo = storedVideos.length === 1 && !Number.isInteger(storedVideos[0].fileIndex);
        const imdbId: string | undefined = PromiseHelpers.mostCommonValue(storedVideos.map(stored => stored.imdbId));
        const kitsuId: number = PromiseHelpers.mostCommonValue(storedVideos.map(stored => stored.kitsuId || 0));

        const fileCollection: ITorrentFileCollection = await this.fileService.parseTorrentFiles(torrent)
            .then(torrentContents => notOpenedVideo ? torrentContents : {...torrentContents, videos: storedVideos.map(video => video.dataValues)})
            .then(torrentContents => this.subtitleService.assignSubtitles(torrentContents))
            .then(torrentContents => this.assignMetaIds(torrentContents, imdbId, kitsuId))
            .catch(error => {
                this.logger.warn(`Failed getting contents for [${torrent.infoHash}] ${torrent.title}`, error.message);
                return {};
            });

        this.assignMetaIds(fileCollection, imdbId, kitsuId);

        if (!fileCollection.contents || !fileCollection.contents.length) {
            return;
        }

        if (notOpenedVideo && fileCollection.videos?.length === 1) {
            // if both have a single video and stored one was not opened, update stored one to true metadata and use that
            storedVideos[0].fileIndex = fileCollection?.videos[0]?.fileIndex || 0;
            storedVideos[0].title = fileCollection.videos[0].title;
            storedVideos[0].size = fileCollection.videos[0].size || 0;
            const subtitles: ISubtitleAttributes[] = fileCollection.videos[0]?.subtitles || [];
            storedVideos[0].subtitles = subtitles.map(subtitle => Subtitle.build(subtitle));
            fileCollection.videos[0] = {...storedVideos[0], subtitles: subtitles};
        }
        // no videos available or more than one new videos were in the torrent
        const shouldDeleteOld = notOpenedVideo && fileCollection.videos?.every(video => !video.id) || false;

        const newTorrent: ITorrentCreationAttributes = {
            ...torrent,
            files: fileCollection.videos,
            contents: fileCollection.contents,
            subtitles: fileCollection.subtitles
        };

        return this.repository.createTorrent(newTorrent)
            .then(() => {
                if (shouldDeleteOld) {
                    this.logger.debug(`Deleting old video for [${torrent.infoHash}] ${torrent.title}`)
                    return storedVideos[0].destroy();
                }
                return Promise.resolve();
            })
            .then(() => PromiseHelpers.sequence(fileCollection.videos!.map(video => (): Promise<void> => {
                const newVideo: IFileCreationAttributes = {...video, infoHash: video.infoHash, title: video.title};
                return this.repository.createFile(newVideo)
            })))
            .then(() => this.logger.info(`Created contents for ${torrent.provider} [${torrent.infoHash}] ${torrent.title}`))
            .catch(error => this.logger.error(`Failed saving contents for [${torrent.infoHash}] ${torrent.title}`, error));
    };

    public updateTorrentSeeders = async (torrent: ITorrentAttributes): Promise<[number]> => {
        if (!(torrent.infoHash || (torrent.provider && torrent.torrentId)) || !Number.isInteger(torrent.seeders)) {
            return [0];
        }
        
        if (torrent.seeders === undefined) {
            this.logger.warn(`Seeders not found for ${torrent.provider} [${torrent.infoHash}] ${torrent.title}`);
            return [0];
        }
        
        
        return this.repository.setTorrentSeeders(torrent, torrent.seeders)
            .catch(error => {
                this.logger.warn('Failed updating seeders:', error);
                return [0];
            });
    };

    private assignMetaIds = (fileCollection: ITorrentFileCollection, imdbId: string | undefined, kitsuId: number): ITorrentFileCollection => {
        if (fileCollection.videos && fileCollection.videos.length) {
            fileCollection.videos.forEach(video => {
                video.imdbId = imdbId || '';
                video.kitsuId = kitsuId || 0;
            });
        }

        return fileCollection;
    };

    private overwriteExistingFiles = async (torrent: IParsedTorrent, torrentContents: ITorrentFileCollection): Promise<ITorrentFileCollection> => {
        const videos = torrentContents && torrentContents.videos;
        if (videos && videos.length) {
            const existingFiles = await this.repository.getFiles(torrent.infoHash)
                .then((existing) => existing.reduce<{ [key: number]: File[] }>((map, next) => {
                    const fileIndex = next.fileIndex !== undefined ? next.fileIndex : null;
                    if (fileIndex !== null) {
                        map[fileIndex] = (map[fileIndex] || []).concat(next);
                    }
                    return map;
                }, {}))
                .catch(() => undefined);
            if (existingFiles && Object.keys(existingFiles).length) {
                const overwrittenVideos = videos
                    .map(file => {
                        const index = file.fileIndex !== undefined ? file.fileIndex : null;
                        let mapping;
                        if (index !== null) {
                            mapping = videos.length === 1 && Object.keys(existingFiles).length === 1
                                ? Object.values(existingFiles)[0]
                                : existingFiles[index];
                        }
                        if (mapping) {
                            const originalFile = mapping.shift();
                            return {id: originalFile!.id, ...file};
                        }
                        return file;
                    });
                return {...torrentContents, videos: overwrittenVideos};
            }
            return torrentContents;
        }
        return Promise.reject(`No video files found for: ${torrent.title}`);
    };
}