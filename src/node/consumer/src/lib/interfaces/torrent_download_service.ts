import {IParsedTorrent} from "@interfaces/parsed_torrent";
import {ITorrentFileCollection} from "@interfaces/torrent_file_collection";

export interface ITorrentDownloadService {
    getTorrentFiles(torrent: IParsedTorrent, timeout: number): Promise<ITorrentFileCollection>;
}
