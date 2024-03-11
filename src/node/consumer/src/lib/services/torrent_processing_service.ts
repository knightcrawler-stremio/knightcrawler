import {TorrentType} from "@enums/torrent_types";
import {ILoggingService} from "@interfaces/logging_service";
import {IParsedTorrent} from "@interfaces/parsed_torrent";
import {ITorrentEntriesService} from "@interfaces/torrent_entries_service";
import {ITorrentProcessingService} from "@interfaces/torrent_processing_service";
import {ITrackerService} from "@interfaces/tracker_service";
import {IIngestedTorrentAttributes} from "@repository/interfaces/ingested_torrent_attributes";
import {IocTypes} from "@setup/ioc_types";
import {inject, injectable} from "inversify";

@injectable()
export class TorrentProcessingService implements ITorrentProcessingService {
    @inject(IocTypes.ITorrentEntriesService) torrentEntriesService: ITorrentEntriesService;
    @inject(IocTypes.ILoggingService) logger: ILoggingService;
    @inject(IocTypes.ITrackerService) trackerService: ITrackerService;

    async processTorrentRecord(torrent: IIngestedTorrentAttributes): Promise<void> {
        const {category} = torrent;
        const type = category === 'tv' ? TorrentType.Series : TorrentType.Movie;
        const torrentInfo: IParsedTorrent = await this.parseTorrent(torrent, type);

        this.logger.info(`Processing torrent ${torrentInfo.title} with infoHash ${torrentInfo.infoHash}`);

        if (await this.torrentEntriesService.checkAndUpdateTorrent(torrentInfo)) {
            return;
        }

        return this.torrentEntriesService.createTorrentEntry(torrentInfo, false);
    }

    private parseTorrent = async (torrent: IIngestedTorrentAttributes, category: string): Promise<IParsedTorrent> => {
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
        }
    };

    private parseImdbId = (torrent: IIngestedTorrentAttributes): string | undefined => {
        if (torrent.imdb === undefined || torrent.imdb === null) {
            return undefined;
        }

        return torrent.imdb;
    };
}
