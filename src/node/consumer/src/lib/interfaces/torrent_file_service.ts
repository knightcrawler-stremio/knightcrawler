import {IParsedTorrent} from "./parsed_torrent";
import {ITorrentFileCollection} from "./torrent_file_collection";

export interface ITorrentFileService {
    parseTorrentFiles(torrent: IParsedTorrent): Promise<ITorrentFileCollection>;

    isPackTorrent(torrent: IParsedTorrent): boolean;
}