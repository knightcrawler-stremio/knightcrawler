import {Optional} from "sequelize";
import {SubtitleAttributes} from "./subtitle_attributes";

export interface FileAttributes {
    id?: number;
    infoHash: string;
    fileIndex: number;
    title: string;
    size: number;
    imdbId: string;
    imdbSeason: number;
    imdbEpisode: number;
    kitsuId: number;
    kitsuEpisode: number;
    subtitles?: SubtitleAttributes[];
}

export interface FileCreationAttributes extends Optional<FileAttributes, 'fileIndex' | 'size' | 'imdbId' | 'imdbSeason' | 'imdbEpisode' | 'kitsuId' | 'kitsuEpisode'> {
}