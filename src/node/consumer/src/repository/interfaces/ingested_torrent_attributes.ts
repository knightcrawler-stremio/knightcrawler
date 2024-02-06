import {Optional} from "sequelize";

export interface IngestedTorrentAttributes {
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

export interface IngestedTorrentCreationAttributes extends Optional<IngestedTorrentAttributes, 'processed'> {
}