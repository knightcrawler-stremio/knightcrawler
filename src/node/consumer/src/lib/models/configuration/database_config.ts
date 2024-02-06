import {BooleanHelpers} from "../../helpers/boolean_helpers";

export const databaseConfig = {
    POSTGRES_HOST: process.env.POSTGRES_HOST || 'postgres',
    POSTGRES_PORT: parseInt(process.env.POSTGRES_PORT || '5432'),
    POSTGRES_DB: process.env.POSTGRES_DB || 'knightcrawler',
    POSTGRES_USER: process.env.POSTGRES_USER || 'postgres',
    POSTGRES_PASSWORD: process.env.POSTGRES_PASSWORD || 'postgres',
    AUTO_CREATE_AND_APPLY_MIGRATIONS: BooleanHelpers.parseBool(process.env.AUTO_CREATE_AND_APPLY_MIGRATIONS, false),

    get POSTGRES_URI() {
        return `postgres://${this.POSTGRES_USER}:${this.POSTGRES_PASSWORD}@${this.POSTGRES_HOST}:${this.POSTGRES_PORT}/${this.POSTGRES_DB}`;
    }
};