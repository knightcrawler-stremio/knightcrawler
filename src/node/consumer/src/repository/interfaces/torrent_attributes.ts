import {Optional} from "sequelize";
import {ContentAttributes} from "./content_attributes";
import {SubtitleAttributes} from "./subtitle_attributes";
import {FileAttributes} from "./file_attributes";

export interface TorrentAttributes {
    infoHash: string;
    provider?: string;
    torrentId?: string;
    title?: string;
    size?: number;
    type?: string;
    uploadDate?: Date;
    seeders?: number;
    trackers?: string;
    languages?: string;
    resolution?: string;
    reviewed?: boolean;
    opened?: boolean;
    contents?: ContentAttributes[];
    files?: FileAttributes[];
    subtitles?: SubtitleAttributes[];
}

export interface TorrentCreationAttributes extends Optional<TorrentAttributes, 'torrentId' | 'size' | 'seeders' | 'trackers' | 'languages' | 'resolution' | 'reviewed' | 'opened'> {
}