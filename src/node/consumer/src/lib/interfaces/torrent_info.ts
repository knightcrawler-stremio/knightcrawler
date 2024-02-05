import {TorrentType} from "../enums/torrent_types";

export interface TorrentInfo {
    title: string | null;
    torrentId: string;
    infoHash: string | null;
    seeders: number;
    uploadDate: Date;
    imdbId?: string;
    kitsuId?: string;
    type: TorrentType;
    provider?: string | null;
    trackers: string;
    size?: number;
    pack?: boolean;
}