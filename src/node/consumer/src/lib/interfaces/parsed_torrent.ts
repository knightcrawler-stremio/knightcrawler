import {IParseTorrentTitleResult} from "./parse_torrent_title_result";
import {TorrentType} from "../enums/torrent_types";
import {ITorrentFileCollection} from "./torrent_file_collection";

export interface IParsedTorrent extends IParseTorrentTitleResult {
    size?: number;
    isPack?: boolean;
    imdbId?: string | number;
    kitsuId?: string | number;
    trackers?: string;
    provider?: string | null;
    infoHash: string | null;
    type: string | TorrentType;
    uploadDate?: Date;
    seeders?: number;
    torrentId?: string;
    fileCollection?: ITorrentFileCollection;
}