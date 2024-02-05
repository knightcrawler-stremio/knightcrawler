import {TorrentInfo} from "../interfaces/torrent_info";
import {TorrentType} from "../enums/torrent_types";
import {logger} from "./logging_service";
import {checkAndUpdateTorrent, createTorrentEntry} from "../torrentEntries.js";
import {trackerService} from "./tracker_service";
import {IngestedTorrentAttributes} from "../../repository/interfaces/ingested_torrent_attributes";

class TorrentProcessingService {
    public async processTorrentRecord(torrent: IngestedTorrentAttributes): Promise<void> {
        const { category } = torrent;
        const type = category === 'tv' ? TorrentType.SERIES : TorrentType.MOVIE;
        const torrentInfo: TorrentInfo = await this.parseTorrent(torrent, type);

        logger.info(`Processing torrent ${torrentInfo.title} with infoHash ${torrentInfo.infoHash}`);

        if (await checkAndUpdateTorrent(torrentInfo)) {
            return;
        }

        return createTorrentEntry(torrentInfo);
    }

    private async assignTorrentTrackers(): Promise<string> {
        const trackers = await trackerService.getTrackers();
        return trackers.join(',');
    }

    private async parseTorrent(torrent: IngestedTorrentAttributes, category: string): Promise<TorrentInfo> {
        const infoHash = torrent.info_hash?.trim().toLowerCase()
        return {
            title: torrent.name,
            torrentId: `${torrent.name}_${infoHash}`,
            infoHash: infoHash,
            seeders: 100,
            size: torrent.size,
            uploadDate: torrent.createdAt,
            imdbId: this.parseImdbId(torrent),
            type: category,
            provider: torrent.source,
            trackers: await this.assignTorrentTrackers(),
        }
    }

    private parseImdbId(torrent: IngestedTorrentAttributes): string | undefined {
        if (torrent.imdb === undefined || torrent.imdb === null) {
            return undefined;
        }

        return torrent.imdb;
    }    
}

export const torrentProcessingService = new TorrentProcessingService();

