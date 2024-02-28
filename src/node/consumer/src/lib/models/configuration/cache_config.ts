import {BooleanHelpers} from "@helpers/boolean_helpers";

export const cacheConfig = {
    MONGODB_HOST: process.env.MONGODB_HOST || 'mongodb',
    MONGODB_PORT: process.env.MONGODB_PORT || '27017',
    MONGODB_DB: process.env.MONGODB_DB || 'knightcrawler',
    MONGODB_USER: process.env.MONGODB_USER || 'mongo',
    MONGODB_PASSWORD: process.env.MONGODB_PASSWORD || 'mongo',
    NO_CACHE: BooleanHelpers.parseBool(process.env.NO_CACHE, false),
    COLLECTION_NAME: process.env.MONGODB_COLLECTION || 'knightcrawler_consumer_collection',

    get MONGO_URI(): string {
        return `mongodb://${this.MONGODB_USER}:${this.MONGODB_PASSWORD}@${this.MONGODB_HOST}:${this.MONGODB_PORT}/${this.MONGODB_DB}?authSource=admin`;
    }
};