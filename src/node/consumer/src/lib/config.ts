const parseBool = (boolString: string | undefined, defaultValue: boolean): boolean =>
    boolString?.toLowerCase() === 'true' ? true : defaultValue;

export const rabbitConfig = {
    RABBIT_URI: process.env.RABBIT_URI || 'amqp://localhost',
    QUEUE_NAME: process.env.QUEUE_NAME || 'test-queue'
}

export const cacheConfig = {
    MONGODB_HOST: process.env.MONGODB_HOST || 'mongodb',
    MONGODB_PORT: process.env.MONGODB_PORT || '27017',
    MONGODB_DB: process.env.MONGODB_DB || 'knightcrawler',
    MONGO_INITDB_ROOT_USERNAME: process.env.MONGO_INITDB_ROOT_USERNAME || 'mongo',
    MONGO_INITDB_ROOT_PASSWORD: process.env.MONGO_INITDB_ROOT_PASSWORD || 'mongo',
    NO_CACHE: parseBool(process.env.NO_CACHE, false),
    COLLECTION_NAME: process.env.MONGODB_COLLECTION || 'knightcrawler_consumer_collection',
    MONGO_URI: '',
}

cacheConfig.MONGO_URI = `mongodb://${cacheConfig.MONGO_INITDB_ROOT_USERNAME}:${cacheConfig.MONGO_INITDB_ROOT_PASSWORD}@${cacheConfig.MONGODB_HOST}:${cacheConfig.MONGODB_PORT}/${cacheConfig.MONGODB_DB}?authSource=admin`;

export const databaseConfig = {
    POSTGRES_HOST: process.env.POSTGRES_HOST || 'postgres',
    POSTGRES_PORT: process.env.POSTGRES_PORT || '5432',
    POSTGRES_DATABASE: process.env.POSTGRES_DATABASE || 'knightcrawler',
    POSTGRES_USERNAME: process.env.POSTGRES_USERNAME || 'postgres',
    POSTGRES_PASSWORD: process.env.POSTGRES_PASSWORD || 'postgres',
    POSTGRES_URI: '',
}

databaseConfig.POSTGRES_URI = `postgres://${databaseConfig.POSTGRES_USERNAME}:${databaseConfig.POSTGRES_PASSWORD}@${databaseConfig.POSTGRES_HOST}:${databaseConfig.POSTGRES_PORT}/${databaseConfig.POSTGRES_DATABASE}`;

export const jobConfig = {
    JOB_CONCURRENCY: Number.parseInt(process.env.JOB_CONCURRENCY || "1", 10),
    JOBS_ENABLED: parseBool(process.env.JOBS_ENABLED, true),
}

export const metadataConfig = {
    IMDB_CONCURRENT: Number.parseInt(process.env.IMDB_CONCURRENT || "1", 10),
    IMDB_INTERVAL_MS: Number.parseInt(process.env.IMDB_INTERVAL_MS || "1000", 10),
}

export const trackerConfig = {
    TRACKERS_URL: process.env.TRACKERS_URL || 'https://ngosang.github.io/trackerslist/trackers_all.txt',
    UDP_ENABLED: parseBool(process.env.UDP_TRACKERS_ENABLED, false),
}

export const torrentConfig = {
    MAX_CONNECTIONS_PER_TORRENT: Number.parseInt(process.env.MAX_SINGLE_TORRENT_CONNECTIONS || "20", 10),
    TIMEOUT: Number.parseInt(process.env.TORRENT_TIMEOUT || "30000", 10),
}