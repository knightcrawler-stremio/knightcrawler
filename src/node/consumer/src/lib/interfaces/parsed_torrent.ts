import {ParseTorrentTitleResult} from "./parse_torrent_title_result";
import {TorrentType} from "../enums/torrent_types";
import {TorrentFileCollection} from "./torrent_file_collection";

export interface ParsedTorrent extends ParseTorrentTitleResult {
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
    fileCollection?: TorrentFileCollection;
}