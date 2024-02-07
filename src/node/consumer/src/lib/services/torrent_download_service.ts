import {encode} from 'magnet-uri';
import torrentStream from 'torrent-stream';
import {configurationService} from './configuration_service';
import {ExtensionHelpers} from '../helpers/extension_helpers';
import {ITorrentFileCollection} from "../interfaces/torrent_file_collection";
import {IParsedTorrent} from "../interfaces/parsed_torrent";
import {IFileAttributes} from "../../repository/interfaces/file_attributes";
import {ISubtitleAttributes} from "../../repository/interfaces/subtitle_attributes";
import {IContentAttributes} from "../../repository/interfaces/content_attributes";
import {parse} from "parse-torrent-title";
import {ITorrentDownloadService} from "../interfaces/torrent_download_service";
import {injectable} from "inversify";

interface ITorrentFile {
    name: string;
    path: string;
    length: number;
    fileIndex: number;
}

@injectable()
export class TorrentDownloadService implements ITorrentDownloadService {
    private engineOptions: TorrentStream.TorrentEngineOptions = {
        connections: configurationService.torrentConfig.MAX_CONNECTIONS_PER_TORRENT,
        uploads: 0,
        verify: false,
        dht: false,
        tracker: true,
    };

    public getTorrentFiles = async (torrent: IParsedTorrent, timeout: number = 30000): Promise<ITorrentFileCollection> => {
        const torrentFiles: ITorrentFile[] = await this.filesFromTorrentStream(torrent, timeout);

        const videos = this.filterVideos(torrent, torrentFiles);
        const subtitles = this.filterSubtitles(torrent, torrentFiles);
        const contents = this.createContent(torrent, torrentFiles);

        return {
            contents: contents,
            videos: videos,
            subtitles: subtitles,
        };
    };

    private filesFromTorrentStream = async (torrent: IParsedTorrent, timeout: number): Promise<ITorrentFile[]> => {
        if (!torrent.infoHash) {
            return Promise.reject(new Error("No infoHash..."));
        }
        const magnet = encode({infoHash: torrent.infoHash, announce: torrent.trackers.split(',')});

        return new Promise((resolve, reject) => {
            let engine: TorrentStream.TorrentEngine;

            const timeoutId = setTimeout(() => {
                engine.destroy(() => {
                });
                reject(new Error('No available connections for torrent!'));
            }, timeout);

            engine = torrentStream(magnet, this.engineOptions);

            engine.on("ready", () => {
                const files: ITorrentFile[] = engine.files.map((file, fileId) => ({
                    ...file,
                    fileIndex: fileId,
                    size: file.length,
                    title: file.name
                }));

                resolve(files);

                engine.destroy(() => {
                });
                clearTimeout(timeoutId);
            });
        });
    };

    private filterVideos = (torrent: IParsedTorrent, torrentFiles: ITorrentFile[]): IFileAttributes[] => {
        if (torrentFiles.length === 1 && !Number.isInteger(torrentFiles[0].fileIndex)) {
            return [this.mapTorrentFileToFileAttributes(torrent, torrentFiles[0])];
        }
        const videos = torrentFiles.filter(file => ExtensionHelpers.isVideo(file.path || ''));
        const maxSize = Math.max(...videos.map((video: ITorrentFile) => video.length));
        const minSampleRatio = videos.length <= 3 ? 3 : 10;
        const minAnimeExtraRatio = 5;
        const minRedundantRatio = videos.length <= 3 ? 30 : Number.MAX_VALUE;

        const isSample = (video: ITorrentFile) => video.path?.match(/sample|bonus|promo/i) && maxSize / parseInt(video.path.toString()) > minSampleRatio;
        const isRedundant = (video: ITorrentFile) => maxSize / parseInt(video.path.toString()) > minRedundantRatio;
        const isExtra = (video: ITorrentFile) => video.path?.match(/extras?\//i);
        const isAnimeExtra = (video: ITorrentFile) => video.path?.match(/(?:\b|_)(?:NC)?(?:ED|OP|PV)(?:v?\d\d?)?(?:\b|_)/i)
            && maxSize / parseInt(video.length.toString()) > minAnimeExtraRatio;
        const isWatermark = (video: ITorrentFile) => video.path?.match(/^[A-Z-]+(?:\.[A-Z]+)?\.\w{3,4}$/)
            && maxSize / parseInt(video.length.toString()) > minAnimeExtraRatio

        return videos
            .filter(video => !isSample(video))
            .filter(video => !isExtra(video))
            .filter(video => !isAnimeExtra(video))
            .filter(video => !isRedundant(video))
            .filter(video => !isWatermark(video))
            .map(video => this.mapTorrentFileToFileAttributes(torrent, video));
    };

    private filterSubtitles = (torrent: IParsedTorrent, torrentFiles: ITorrentFile[]): ISubtitleAttributes[] => torrentFiles.filter(file => ExtensionHelpers.isSubtitle(file.name || ''))
        .map(file => this.mapTorrentFileToSubtitleAttributes(torrent, file));

    private createContent = (torrent: IParsedTorrent, torrentFiles: ITorrentFile[]): IContentAttributes[] => torrentFiles.map(file => this.mapTorrentFileToContentAttributes(torrent, file));

    private mapTorrentFileToFileAttributes = (torrent: IParsedTorrent, file: ITorrentFile): IFileAttributes => {
        const videoFile: IFileAttributes = {
            title: file.name,
            size: file.length,
            fileIndex: file.fileIndex || 0,
            infoHash: torrent.infoHash,
            imdbId: torrent.imdbId.toString(),
            imdbSeason: torrent.season || 0,
            imdbEpisode: torrent.episode || 0,
            kitsuId: parseInt(torrent.kitsuId.toString()) || 0,
            kitsuEpisode: torrent.episode || 0
        };
        
        return {...videoFile, ...parse(file.name)};
    };

    private mapTorrentFileToSubtitleAttributes = (torrent: IParsedTorrent, file: ITorrentFile): ISubtitleAttributes => ({
        title: file.name,
        infoHash: torrent.infoHash,
        fileIndex: file.fileIndex,
        fileId: file.fileIndex,
        path: file.path,
    });

    private mapTorrentFileToContentAttributes = (torrent: IParsedTorrent, file: ITorrentFile): IContentAttributes => ({
        infoHash: torrent.infoHash,
        fileIndex: file.fileIndex,
        path: file.path,
        size: file.length,
    });
}

