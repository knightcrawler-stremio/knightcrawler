import {IParsedTorrent} from "./parsed_torrent";
import {ITorrentFileCollection} from "./torrent_file_collection";

export interface ITorrentDownloadService {
    getTorrentFiles(torrent: IParsedTorrent, timeout: number): Promise<ITorrentFileCollection>;
}