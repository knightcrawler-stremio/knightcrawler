import {Optional} from "sequelize";

export interface ContentAttributes {
    infoHash: string;
    fileIndex: number;
    path: string;
    size: number;
}

export interface ContentCreationAttributes extends Optional<ContentAttributes, 'fileIndex' | 'size'> {
}