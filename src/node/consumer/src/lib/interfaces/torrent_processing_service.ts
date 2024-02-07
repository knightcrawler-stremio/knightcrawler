import {IIngestedTorrentAttributes} from "../../repository/interfaces/ingested_torrent_attributes";

export interface ITorrentProcessingService {
    processTorrentRecord(torrent: IIngestedTorrentAttributes): Promise<void>;
}