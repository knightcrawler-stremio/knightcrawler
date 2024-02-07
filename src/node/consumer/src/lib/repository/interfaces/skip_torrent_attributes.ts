import {Optional} from "sequelize";

export interface ISkipTorrentAttributes {
    infoHash: string;
}

export interface ISkipTorrentCreationAttributes extends Optional<ISkipTorrentAttributes, never> {
}