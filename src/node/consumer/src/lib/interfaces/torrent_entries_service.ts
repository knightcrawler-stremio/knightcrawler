import {IParsedTorrent} from "./parsed_torrent";
import {Torrent} from "../../repository/models/torrent";
import {ITorrentAttributes} from "../../repository/interfaces/torrent_attributes";
import {SkipTorrent} from "../../repository/models/skipTorrent";

export interface ITorrentEntriesService {
    createTorrentEntry(torrent: IParsedTorrent, overwrite): Promise<void>;

    createSkipTorrentEntry(torrent: Torrent): Promise<[SkipTorrent, boolean]>;

    getStoredTorrentEntry(torrent: Torrent): Promise<SkipTorrent | Torrent>;

    checkAndUpdateTorrent(torrent: IParsedTorrent): Promise<boolean>;

    createTorrentContents(torrent: Torrent): Promise<void>;

    updateTorrentSeeders(torrent: ITorrentAttributes): Promise<[number] | ITorrentAttributes>;
}