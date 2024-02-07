import {Optional} from "sequelize";

export interface IProviderAttributes {
    name: string;
    lastScraped: Date;
    lastScrapedId: string;
}

export interface IProviderCreationAttributes extends Optional<IProviderAttributes, 'lastScraped' | 'lastScrapedId'> {
}