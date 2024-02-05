import {Optional} from "sequelize";

export interface SkipTorrentAttributes {
    infoHash: string;
}

export interface SkipTorrentCreationAttributes extends Optional<SkipTorrentAttributes, never> {
}