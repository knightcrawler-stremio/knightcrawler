import {inject, injectable} from "inversify";
import {TorrentType} from "../enums/torrent_types";
import {ILoggingService} from "../interfaces/logging_service";
import {IParsedTorrent} from "../interfaces/parsed_torrent";
import {ITorrentEntriesService} from "../interfaces/torrent_entries_service";
import {ITorrentProcessingService} from "../interfaces/torrent_processing_service";
import {ITrackerService} from "../interfaces/tracker_service";
import {IocTypes} from "../models/ioc_types";
import {IIngestedTorrentAttributes} from "../repository/interfaces/ingested_torrent_attributes";

@injectable()
export class TorrentProcessingService implements ITorrentProcessingService {
    private torrentEntriesService: ITorrentEntriesService;
    private logger: ILoggingService;
    private trackerService: ITrackerService;

    constructor(@inject(IocTypes.ITorrentEntriesService) torrentEntriesService: ITorrentEntriesService,
                @inject(IocTypes.ILoggingService) logger: ILoggingService,
                @inject(IocTypes.ITrackerService) trackerService: ITrackerService) {
        this.torrentEntriesService = torrentEntriesService;
        this.logger = logger;
        this.trackerService = trackerService;
    }

    public processTorrentRecord = async (torrent: IIngestedTorrentAttributes): Promise<void> => {
        const {category} = torrent;
        const type = category === 'tv' ? TorrentType.Series : TorrentType.Movie;
        const torrentInfo: IParsedTorrent = await this.parseTorrent(torrent, type);

        this.logger.info(`Processing torrent ${torrentInfo.title} with infoHash ${torrentInfo.infoHash}`);

        if (await this.torrentEntriesService.checkAndUpdateTorrent(torrentInfo)) {
            return;
        }

        return this.torrentEntriesService.createTorrentEntry(torrentInfo, false);
    };

    private assignTorrentTrackers = async (): Promise<string> => {
        const trackers = await this.trackerService.getTrackers();
        return trackers.join(',');
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
            trackers: await this.assignTorrentTrackers(),
        }
    };

    private parseImdbId = (torrent: IIngestedTorrentAttributes): string | undefined => {
        if (torrent.imdb === undefined || torrent.imdb === null) {
            return undefined;
        }

        return torrent.imdb;
    };
}

