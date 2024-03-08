import {IContentAttributes} from "@repository/interfaces/content_attributes";
import {IFileAttributes} from "@repository/interfaces/file_attributes";
import {ISubtitleAttributes} from "@repository/interfaces/subtitle_attributes";
import {Optional} from "sequelize";

export interface ITorrentAttributes {
    infoHash: string;
    provider?: string | null;
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
