import {Optional} from "sequelize";
import {IContentAttributes} from "./content_attributes";
import {ISubtitleAttributes} from "./subtitle_attributes";
import {IFileAttributes} from "./file_attributes";

export interface ITorrentAttributes {
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
    contents?: IContentAttributes[];
    files?: IFileAttributes[];
    subtitles?: ISubtitleAttributes[];
}

export interface ITorrentCreationAttributes extends Optional<ITorrentAttributes, 'torrentId' | 'size' | 'seeders' | 'trackers' | 'languages' | 'resolution' | 'reviewed' | 'opened'> {
}