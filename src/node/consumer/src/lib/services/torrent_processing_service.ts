import {TorrentType} from "../enums/torrent_types";
import {logger} from "./logging_service";
import {trackerService} from "./tracker_service";
import {torrentEntriesService} from "./torrent_entries_service";
import {IngestedTorrentAttributes} from "../../repository/interfaces/ingested_torrent_attributes";
import {ParsedTorrent} from "../interfaces/parsed_torrent";

class TorrentProcessingService {
    public async processTorrentRecord(torrent: IngestedTorrentAttributes): Promise<void> {
        const { category } = torrent;
        const type = category === 'tv' ? TorrentType.Series : TorrentType.Movie;
        const torrentInfo: ParsedTorrent = await this.parseTorrent(torrent, type);

        logger.info(`Processing torrent ${torrentInfo.title} with infoHash ${torrentInfo.infoHash}`);

        if (await torrentEntriesService.checkAndUpdateTorrent(torrentInfo)) {
            return;
        }

        return torrentEntriesService.createTorrentEntry(torrentInfo);
    }

    private async assignTorrentTrackers(): Promise<string> {
        const trackers = await trackerService.getTrackers();
        return trackers.join(',');
    }

    private async parseTorrent(torrent: IngestedTorrentAttributes, category: string): Promise<ParsedTorrent> {
        const infoHash = torrent.info_hash?.trim().toLowerCase()
        return {
            title: torrent.name,
            torrentId: `${torrent.name}_${infoHash}`,
            infoHash: infoHash,
            seeders: 100,
            size: parseInt(torrent.size),
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

