import {BooleanHelpers} from "@helpers/boolean_helpers";

export const cacheConfig = {
    MONGODB_HOST: process.env.MONGODB_HOST || 'mongodb',
    MONGODB_PORT: process.env.MONGODB_PORT || '27017',
    MONGODB_DB: process.env.MONGODB_DB || 'knightcrawler',
    MONGO_INITDB_ROOT_USERNAME: process.env.MONGO_INITDB_ROOT_USERNAME || 'mongo',
    MONGO_INITDB_ROOT_PASSWORD: process.env.MONGO_INITDB_ROOT_PASSWORD || 'mongo',
    NO_CACHE: BooleanHelpers.parseBool(process.env.NO_CACHE, false),
    COLLECTION_NAME: process.env.MONGODB_COLLECTION || 'knightcrawler_consumer_collection',

    get MONGO_URI(): string {
        return `mongodb://${this.MONGO_INITDB_ROOT_USERNAME}:${this.MONGO_INITDB_ROOT_PASSWORD}@${this.MONGODB_HOST}:${this.MONGODB_PORT}/${this.MONGODB_DB}?authSource=admin`;
    }
};