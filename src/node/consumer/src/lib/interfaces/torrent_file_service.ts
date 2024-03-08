import {IParsedTorrent} from "@interfaces/parsed_torrent";
import {ITorrentFileCollection} from "@interfaces/torrent_file_collection";

export interface ITorrentFileService {
    parseTorrentFiles(torrent: IParsedTorrent): Promise<ITorrentFileCollection>;

    isPackTorrent(torrent: IParsedTorrent): boolean;
}
