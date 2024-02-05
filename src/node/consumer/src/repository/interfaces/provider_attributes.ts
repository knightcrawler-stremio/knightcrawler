import {Optional} from "sequelize";

export interface ProviderAttributes {
    name: string;
    lastScraped: Date;
    lastScrapedId: string;
}

export interface ProviderCreationAttributes extends Optional<ProviderAttributes, 'lastScraped' | 'lastScrapedId'> {
}