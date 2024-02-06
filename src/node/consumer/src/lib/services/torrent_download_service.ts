import {encode} from 'magnet-uri';
import torrentStream from 'torrent-stream';
import {configurationService} from './configuration_service';
import {extensionService} from './extension_service';
import {TorrentFileCollection} from "../interfaces/torrent_file_collection";
import {ParsedTorrent} from "../interfaces/parsed_torrent";
import {FileAttributes} from "../../repository/interfaces/file_attributes";
import {SubtitleAttributes} from "../../repository/interfaces/subtitle_attributes";
import {ContentAttributes} from "../../repository/interfaces/content_attributes";
import {parse} from "parse-torrent-title";

interface TorrentFile {
    name: string;
    path: string;
    length: number;
    fileIndex: number;
}

class TorrentDownloadService {
    private engineOptions: TorrentStream.TorrentEngineOptions = {
        connections: configurationService.torrentConfig.MAX_CONNECTIONS_PER_TORRENT,
        uploads: 0,
        verify: false,
        dht: false,
        tracker: true,
    };

    public async getTorrentFiles(torrent: ParsedTorrent, timeout: number = 30000): Promise<TorrentFileCollection> {
        const torrentFiles: TorrentFile[] = await this.filesFromTorrentStream(torrent, timeout);

        const videos = this.filterVideos(torrent, torrentFiles);
        const subtitles = this.filterSubtitles(torrent, torrentFiles);
        const contents = this.createContent(torrent, torrentFiles);

        return {
            contents: contents,
            videos: videos,
            subtitles: subtitles,
        };
    }

    private async filesFromTorrentStream(torrent: ParsedTorrent, timeout: number): Promise<TorrentFile[]> {
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
                const files: TorrentFile[] = engine.files.map((file, fileId) => ({
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
    }

    private filterVideos(torrent: ParsedTorrent, torrentFiles: TorrentFile[]): FileAttributes[] {
        if (torrentFiles.length === 1 && !Number.isInteger(torrentFiles[0].fileIndex)) {
            return [this.mapTorrentFileToFileAttributes(torrent, torrentFiles[0])];
        }
        const videos = torrentFiles.filter(file => extensionService.isVideo(file.path || ''));
        const maxSize = Math.max(...videos.map((video: TorrentFile) => video.length));
        const minSampleRatio = videos.length <= 3 ? 3 : 10;
        const minAnimeExtraRatio = 5;
        const minRedundantRatio = videos.length <= 3 ? 30 : Number.MAX_VALUE;

        const isSample = (video: TorrentFile) => video.path?.match(/sample|bonus|promo/i) && maxSize / parseInt(video.path.toString()) > minSampleRatio;
        const isRedundant = (video: TorrentFile) => maxSize / parseInt(video.path.toString()) > minRedundantRatio;
        const isExtra = (video: TorrentFile) => video.path?.match(/extras?\//i);
        const isAnimeExtra = (video: TorrentFile) => video.path?.match(/(?:\b|_)(?:NC)?(?:ED|OP|PV)(?:v?\d\d?)?(?:\b|_)/i)
            && maxSize / parseInt(video.length.toString()) > minAnimeExtraRatio;
        const isWatermark = (video: TorrentFile) => video.path?.match(/^[A-Z-]+(?:\.[A-Z]+)?\.\w{3,4}$/)
            && maxSize / parseInt(video.length.toString()) > minAnimeExtraRatio

        return videos
            .filter(video => !isSample(video))
            .filter(video => !isExtra(video))
            .filter(video => !isAnimeExtra(video))
            .filter(video => !isRedundant(video))
            .filter(video => !isWatermark(video))
            .map(video => this.mapTorrentFileToFileAttributes(torrent, video));
    }

    private filterSubtitles(torrent: ParsedTorrent, torrentFiles: TorrentFile[]): SubtitleAttributes[] {
        return torrentFiles.filter(file => extensionService.isSubtitle(file.name || ''))
            .map(file => this.mapTorrentFileToSubtitleAttributes(torrent, file));
    }

    private createContent(torrent: ParsedTorrent, torrentFiles: TorrentFile[]): ContentAttributes[] {
        return torrentFiles.map(file => this.mapTorrentFileToContentAttributes(torrent, file));
    }

    private mapTorrentFileToFileAttributes(torrent: ParsedTorrent, file: TorrentFile): FileAttributes {
        const videoFile: FileAttributes = {
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
    }

    private mapTorrentFileToSubtitleAttributes(torrent: ParsedTorrent, file: TorrentFile): SubtitleAttributes {
        return {
            title: file.name,
            infoHash: torrent.infoHash,
            fileIndex: file.fileIndex,
            fileId: file.fileIndex,
            path: file.path,
        };
    }

    private mapTorrentFileToContentAttributes(torrent: ParsedTorrent, file: TorrentFile): ContentAttributes {
        return {
            infoHash: torrent.infoHash,
            fileIndex: file.fileIndex,
            path: file.path,
            size: file.length,
        };
    }
}

export const torrentDownloadService = new TorrentDownloadService();

