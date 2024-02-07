import {ITorrentAttributes} from "../repository/interfaces/torrent_attributes";
import {SkipTorrent} from "../repository/models/skipTorrent";
import {Torrent} from "../repository/models/torrent";
import {IParsedTorrent} from "./parsed_torrent";

export interface ITorrentEntriesService {
    createTorrentEntry(torrent: IParsedTorrent, overwrite: boolean): Promise<void>;

    createSkipTorrentEntry(torrent: Torrent): Promise<[SkipTorrent, boolean | null]>;

    getStoredTorrentEntry(torrent: Torrent): Promise<Torrent | SkipTorrent | null | undefined>;

    checkAndUpdateTorrent(torrent: IParsedTorrent): Promise<boolean>;

    createTorrentContents(torrent: Torrent): Promise<void>;

    updateTorrentSeeders(torrent: ITorrentAttributes): Promise<[number] | undefined>;
}