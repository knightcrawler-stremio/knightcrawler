import {ParseTorrentTitleResult} from "./parse_torrent_title_result";

export interface ParsableTorrentFile extends ParseTorrentTitleResult {
    name?: string;
    path?: string;
    size?: number;
    length?: number;
    fileIndex?: number;
    isMovie?: boolean;
    imdbId?: string | number;
    kitsuId?: number | string;
}