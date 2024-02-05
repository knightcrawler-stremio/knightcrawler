import {Optional} from "sequelize";

export interface SubtitleAttributes {
    infoHash: string;
    fileIndex: number;
    fileId?: number;
    title: string;
    path: string;
}

export interface SubtitleCreationAttributes extends Optional<SubtitleAttributes, 'fileId'> {
}