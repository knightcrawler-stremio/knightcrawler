import {TorrentType} from "@enums/torrent_types";
import {IParseTorrentTitleResult} from "@interfaces/parse_torrent_title_result";
import {ITorrentFileCollection} from "@interfaces/torrent_file_collection";

export interface IParsedTorrent extends IParseTorrentTitleResult {
    size?: number;
    isPack?: boolean;
    imdbId?: string | number;
    kitsuId?: number;
    trackers?: string;
    provider?: string | null;
    infoHash: string;
    type: string | TorrentType;
    uploadDate?: Date;
    seeders?: number;
    torrentId?: string;
    fileCollection?: ITorrentFileCollection;
}