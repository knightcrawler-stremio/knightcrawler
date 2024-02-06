import {parse} from 'parse-torrent-title';
import {ParsedTorrent} from "../interfaces/parsed_torrent";
import {repository} from '../../repository/database_repository';
import {TorrentType} from '../enums/torrent_types';
import {TorrentFileCollection} from "../interfaces/torrent_file_collection";
import {Torrent} from "../../repository/models/torrent";
import {PromiseHelpers} from '../helpers/promises_helpers';
import {logger} from './logging_service';
import {metadataService} from './metadata_service';
import {torrentFileService} from './torrent_file_service';
import {torrentSubtitleService} from './torrent_subtitle_service';
import {TorrentAttributes} from "../../repository/interfaces/torrent_attributes";
import {File} from "../../repository/models/file";
import {Subtitle} from "../../repository/models/subtitle";

class TorrentEntriesService {
    public async createTorrentEntry(torrent: ParsedTorrent, overwrite = false): Promise<void> {
        const titleInfo = parse(torrent.title);

        if (!torrent.imdbId && torrent.type !== TorrentType.Anime) {
            const imdbQuery = {
                title: titleInfo.title,
                year: titleInfo.year,
                type: torrent.type
            };
            torrent.imdbId = await metadataService.getImdbId(imdbQuery)
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
            torrent.kitsuId = await metadataService.getKitsuId(kitsuQuery)
                .catch(() => undefined);
        }

        if (!torrent.imdbId && !torrent.kitsuId && !torrentFileService.isPackTorrent(torrent)) {
            logger.warn(`imdbId or kitsuId not found:  ${torrent.provider} ${torrent.title}`);
            return;
        }

        const fileCollection: TorrentFileCollection = await torrentFileService.parseTorrentFiles(torrent)
            .then((torrentContents: TorrentFileCollection) => overwrite ? this.overwriteExistingFiles(torrent, torrentContents) : torrentContents)
            .then((torrentContents: TorrentFileCollection) => torrentSubtitleService.assignSubtitles(torrentContents))
            .catch(error => {
                logger.warn(`Failed getting files for ${torrent.title}`, error.message);
                return {};
            });

        if (!fileCollection.videos || !fileCollection.videos.length) {
            logger.warn(`no video files found for ${torrent.provider} [${torrent.infoHash}] ${torrent.title}`);
            return;
        }

        const newTorrent: Torrent = Torrent.build({
            ...torrent,
            contents: fileCollection.contents,
            subtitles: fileCollection.subtitles
        });

        return repository.createTorrent(newTorrent)
            .then(() => PromiseHelpers.sequence(fileCollection.videos.map(video => () => {
                const newVideo = File.build(video);
                return repository.createFile(newVideo)
            })))
            .then(() => logger.info(`Created ${torrent.provider} entry for [${torrent.infoHash}] ${torrent.title}`));
    }

    public async createSkipTorrentEntry(torrent: Torrent) {
        return repository.createSkipTorrent(torrent);
    }

    public async getStoredTorrentEntry(torrent: Torrent) {
        return repository.getSkipTorrent(torrent.infoHash)
            .catch(() => repository.getTorrent(torrent))
            .catch(() => undefined);
    }

    public async checkAndUpdateTorrent(torrent: ParsedTorrent): Promise<boolean> {
        const query: TorrentAttributes = {
            infoHash: torrent.infoHash,
            provider: torrent.provider,
        }

        const existingTorrent = await repository.getTorrent(query).catch(() => undefined);

        if (!existingTorrent) {
            return false;
        }

        if (existingTorrent.provider === 'RARBG') {
            return true;
        }
        if (existingTorrent.provider === 'KickassTorrents' && torrent.provider) {
            existingTorrent.provider = torrent.provider;
            existingTorrent.torrentId = torrent.torrentId;
        }

        if (!existingTorrent.languages && torrent.languages && existingTorrent.provider !== 'RARBG') {
            existingTorrent.languages = torrent.languages;
            await existingTorrent.save();
            logger.debug(`Updated [${existingTorrent.infoHash}] ${existingTorrent.title} language to ${torrent.languages}`);
        }
        return this.createTorrentContents(existingTorrent)
            .then(() => this.updateTorrentSeeders(existingTorrent));
    }

    public async createTorrentContents(torrent: Torrent) {
        if (torrent.opened) {
            return;
        }

        const storedVideos: File[] = await repository.getFiles(torrent.infoHash).catch(() => []);
        if (!storedVideos || !storedVideos.length) {
            return;
        }
        const notOpenedVideo = storedVideos.length === 1 && !Number.isInteger(storedVideos[0].fileIndex);
        const imdbId: string | undefined = PromiseHelpers.mostCommonValue(storedVideos.map(stored => stored.imdbId));
        const kitsuId: number | undefined = PromiseHelpers.mostCommonValue(storedVideos.map(stored => stored.kitsuId));

        const fileCollection: TorrentFileCollection = await torrentFileService.parseTorrentFiles(torrent)
            .then(torrentContents => notOpenedVideo ? torrentContents : {...torrentContents, videos: storedVideos})
            .then(torrentContents => torrentSubtitleService.assignSubtitles(torrentContents))
            .then(torrentContents => this.assignMetaIds(torrentContents, imdbId, kitsuId))
            .catch(error => {
                logger.warn(`Failed getting contents for [${torrent.infoHash}] ${torrent.title}`, error.message);
                return {};
            });

        this.assignMetaIds(fileCollection, imdbId, kitsuId);

        if (!fileCollection.contents || !fileCollection.contents.length) {
            return;
        }

        if (notOpenedVideo && fileCollection.videos.length === 1) {
            // if both have a single video and stored one was not opened, update stored one to true metadata and use that
            storedVideos[0].fileIndex = fileCollection.videos[0].fileIndex;
            storedVideos[0].title = fileCollection.videos[0].title;
            storedVideos[0].size = fileCollection.videos[0].size;
            storedVideos[0].subtitles = fileCollection.videos[0].subtitles.map(subtitle => Subtitle.build(subtitle));
            fileCollection.videos[0] = storedVideos[0];
        }
        // no videos available or more than one new videos were in the torrent
        const shouldDeleteOld = notOpenedVideo && fileCollection.videos.every(video => !video.id);

        const newTorrent: Torrent = Torrent.build({
            ...torrent,
            contents: fileCollection.contents,
            subtitles: fileCollection.subtitles
        });

        return repository.createTorrent(newTorrent)
            .then(() => {
                if (shouldDeleteOld) {
                    logger.debug(`Deleting old video for [${torrent.infoHash}] ${torrent.title}`)
                    return storedVideos[0].destroy();
                }
                return Promise.resolve();
            })
            .then(() => PromiseHelpers.sequence(fileCollection.videos.map(video => () => {
                const newVideo = File.build(video);
                return repository.createFile(newVideo)
            })))
            .then(() => logger.info(`Created contents for ${torrent.provider} [${torrent.infoHash}] ${torrent.title}`))
            .catch(error => logger.error(`Failed saving contents for [${torrent.infoHash}] ${torrent.title}`, error));
    }

    public async updateTorrentSeeders(torrent: TorrentAttributes) {
        if (!(torrent.infoHash || (torrent.provider && torrent.torrentId)) || !Number.isInteger(torrent.seeders)) {
            return torrent;
        }

        return repository.setTorrentSeeders(torrent, torrent.seeders)
            .catch(error => {
                logger.warn('Failed updating seeders:', error);
                return undefined;
            });
    }

    private assignMetaIds(fileCollection: TorrentFileCollection, imdbId: string, kitsuId: number): TorrentFileCollection {
        if (fileCollection.videos && fileCollection.videos.length) {
            fileCollection.videos.forEach(video => {
                video.imdbId = imdbId;
                video.kitsuId = kitsuId;
            });
        }

        return fileCollection;
    }

    private async overwriteExistingFiles(torrent: ParsedTorrent, torrentContents: TorrentFileCollection) {
        const videos = torrentContents && torrentContents.videos;
        if (videos && videos.length) {
            const existingFiles = await repository.getFiles(torrent.infoHash)
                .then((existing) => existing
                    .reduce((map, next) => {
                        const fileIndex = next.fileIndex !== undefined ? next.fileIndex : null;
                        map[fileIndex] = (map[fileIndex] || []).concat(next);
                        return map;
                    }, {}))
                .catch(() => undefined);
            if (existingFiles && Object.keys(existingFiles).length) {
                const overwrittenVideos = videos
                    .map(file => {
                        const mapping = videos.length === 1 && Object.keys(existingFiles).length === 1
                            ? Object.values(existingFiles)[0]
                            : existingFiles[file.fileIndex !== undefined ? file.fileIndex : null];
                        if (mapping) {
                            const originalFile = mapping.shift();
                            return {id: originalFile.id, ...file};
                        }
                        return file;
                    });
                return {...torrentContents, videos: overwrittenVideos};
            }
            return torrentContents;
        }
        return Promise.reject(`No video files found for: ${torrent.title}`);
    }
}

export const torrentEntriesService = new TorrentEntriesService();