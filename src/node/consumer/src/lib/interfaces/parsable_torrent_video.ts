import {ParseTorrentTitleResult} from "./parse_torrent_title_result";

export interface ParsableTorrentVideo extends ParseTorrentTitleResult {
    name: string;
    path: string;
}