import {Optional} from "sequelize";

export interface IContentAttributes {
    infoHash: string;
    fileIndex: number;
    path: string;
    size: number;
}

export interface IContentCreationAttributes extends Optional<IContentAttributes, 'fileIndex' | 'size'> {
}