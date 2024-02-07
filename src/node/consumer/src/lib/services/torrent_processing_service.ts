import {TorrentType} from "../enums/torrent_types";
import {logger} from "./logging_service";
import {trackerService} from "./tracker_service";
import {torrentEntriesService} from "./torrent_entries_service";
import {IIngestedTorrentAttributes} from "../../repository/interfaces/ingested_torrent_attributes";
import {IParsedTorrent} from "../interfaces/parsed_torrent";
import {ITorrentProcessingService} from "../interfaces/torrent_processing_service";

class TorrentProcessingService implements ITorrentProcessingService {
    public async processTorrentRecord(torrent: IIngestedTorrentAttributes): Promise<void> {
        const {category} = torrent;
        const type = category === 'tv' ? TorrentType.Series : TorrentType.Movie;
        const torrentInfo: IParsedTorrent = await this.parseTorrent(torrent, type);

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

    private async parseTorrent(torrent: IIngestedTorrentAttributes, category: string): Promise<IParsedTorrent> {
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

    private parseImdbId(torrent: IIngestedTorrentAttributes): string | undefined {
        if (torrent.imdb === undefined || torrent.imdb === null) {
            return undefined;
        }

        return torrent.imdb;
    }    
}

export const torrentProcessingService = new TorrentProcessingService();

