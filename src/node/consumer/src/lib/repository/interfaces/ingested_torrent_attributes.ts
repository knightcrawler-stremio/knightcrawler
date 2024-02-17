import {Optional} from "sequelize";

export interface IIngestedTorrentAttributes {
    name: string;
    source: string;
    category: string;
    info_hash: string;
    size: string;
    seeders: number;
    leechers: number;
    imdb: string;
    processed: boolean;
    createdAt?: Date;
}

export interface IIngestedTorrentCreationAttributes extends Optional<IIngestedTorrentAttributes, 'processed'> {
}