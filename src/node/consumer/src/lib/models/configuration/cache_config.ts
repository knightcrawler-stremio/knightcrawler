import {BooleanHelpers} from "../../helpers/boolean_helpers";

export class CacheConfig {
    public MONGODB_HOST: string = process.env.MONGODB_HOST || 'mongodb';
    public MONGODB_PORT: string = process.env.MONGODB_PORT || '27017';
    public MONGODB_DB: string = process.env.MONGODB_DB || 'knightcrawler';
    public MONGO_INITDB_ROOT_USERNAME: string = process.env.MONGO_INITDB_ROOT_USERNAME || 'mongo';
    public MONGO_INITDB_ROOT_PASSWORD: string = process.env.MONGO_INITDB_ROOT_PASSWORD || 'mongo';
    public NO_CACHE: boolean = BooleanHelpers.parseBool(process.env.NO_CACHE, false);
    public COLLECTION_NAME: string = process.env.MONGODB_COLLECTION || 'knightcrawler_consumer_collection';
    
    public get MONGO_URI() {
        return `mongodb://${this.MONGO_INITDB_ROOT_USERNAME}:${this.MONGO_INITDB_ROOT_PASSWORD}@${this.MONGODB_HOST}:${this.MONGODB_PORT}/${this.MONGODB_DB}?authSource=admin`;
    }
}