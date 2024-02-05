import { encode } from 'magnet-uri';
import torrentStream from 'torrent-stream';
import { torrentConfig } from '../config';
import {extensionService} from './extension_service';
import {TorrentInfo} from "../interfaces/torrent_info";
import {DownloadedTorrentFile} from "../interfaces/downloaded_torrent_file";

class TorrentDownloadService {
    private engineOptions: TorrentStream.TorrentEngineOptions = {
        connections: torrentConfig.MAX_CONNECTIONS_PER_TORRENT,
        uploads: 0,
        verify: false,
        dht: false,
        tracker: true,
    };
    
    public async getTorrentFiles(torrent: TorrentInfo, timeout: number = 30000): Promise<{ contents: Array<DownloadedTorrentFile>; videos: Array<DownloadedTorrentFile>; subtitles: Array<DownloadedTorrentFile> }> {
        return this.filesFromTorrentStream(torrent, timeout)
            .then((files: Array<DownloadedTorrentFile>) => ({
                contents: files,
                videos: this.filterVideos(files),
                subtitles: this.filterSubtitles(files)
            }));
    }

    private async filesFromTorrentStream(torrent: TorrentInfo, timeout: number): Promise<Array<DownloadedTorrentFile>> {
        if (!torrent.infoHash) {
            return Promise.reject(new Error("No infoHash..."));
        }
        const magnet = encode({ infoHash: torrent.infoHash, announce: torrent.trackers.split(',') });

        return new Promise((resolve, reject) => {
            let engine: TorrentStream.TorrentEngine;

            const timeoutId = setTimeout(() => {
                engine.destroy(() => {});
                reject(new Error('No available connections for torrent!'));
            }, timeout);

            engine = torrentStream(magnet, this.engineOptions);

            engine.on("ready", () => {
                const files: DownloadedTorrentFile[] = engine.files.map((file, fileId) => ({ ...file, fileIndex: fileId }));
                resolve(files);

                engine.destroy(() => {});
                clearTimeout(timeoutId);
            });
        });
    }

    private filterVideos(files: Array<DownloadedTorrentFile>): Array<DownloadedTorrentFile> {
        if (files.length === 1 && !Number.isInteger(files[0].fileIndex)) {
            return files;
        }
        const videos = files.filter(file => extensionService.isVideo(file.path || ''));
        const maxSize = Math.max(...videos.map((video: DownloadedTorrentFile) => video.length));
        const minSampleRatio = videos.length <= 3 ? 3 : 10;
        const minAnimeExtraRatio = 5;
        const minRedundantRatio = videos.length <= 3 ? 30 : Number.MAX_VALUE;
        const isSample = (video: DownloadedTorrentFile) => video.path?.match(/sample|bonus|promo/i) && maxSize / parseInt(video.length.toString()) > minSampleRatio;
        const isRedundant = (video: DownloadedTorrentFile) => maxSize / parseInt(video.length.toString()) > minRedundantRatio;
        const isExtra = (video: DownloadedTorrentFile) => video.path?.match(/extras?\//i);
        const isAnimeExtra = (video: DownloadedTorrentFile) => video.path?.match(/(?:\b|_)(?:NC)?(?:ED|OP|PV)(?:v?\d\d?)?(?:\b|_)/i)
            && maxSize / parseInt(video.length.toString()) > minAnimeExtraRatio;
        const isWatermark = (video: DownloadedTorrentFile) => video.path?.match(/^[A-Z-]+(?:\.[A-Z]+)?\.\w{3,4}$/)
            && maxSize / parseInt(video.length.toString()) > minAnimeExtraRatio
        return videos
            .filter(video => !isSample(video))
            .filter(video => !isExtra(video))
            .filter(video => !isAnimeExtra(video))
            .filter(video => !isRedundant(video))
            .filter(video => !isWatermark(video));
    }

    private filterSubtitles(files: Array<DownloadedTorrentFile>): Array<DownloadedTorrentFile> {
        return files.filter(file => extensionService.isSubtitle(file.path || ''));
    }
}

export const torrentDownloadService = new TorrentDownloadService();

