import {TorrentType} from "../enums/torrent_types";

export interface ParsableTorrent {
    title: string;
    type: TorrentType;
    size: number;
    pack?: boolean;
}

