import {ParsableTorrentFile} from "./parsable_torrent_file";

export interface TorrentFileCollection {
    contents?: ParsableTorrentFile[];
    videos?: ParsableTorrentFile[];
    subtitles?: ParsableTorrentFile[];
}