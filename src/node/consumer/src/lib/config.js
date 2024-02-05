export const rabbitConfig = {
    URI: process.env.RABBIT_URI || 'amqp://localhost',
    QUEUE_NAME: process.env.QUEUE_NAME || 'test-queue'
}

export const cacheConfig = {
    MONGODB_HOST: process.env.MONGODB_HOST || 'mongodb',
    MONGODB_PORT: process.env.MONGODB_PORT || '27017',
    MONGODB_DB: process.env.MONGODB_DB || 'knightcrawler',
    MONGO_INITDB_ROOT_USERNAME: process.env.MONGO_INITDB_ROOT_USERNAME || 'mongo',
    MONGO_INITDB_ROOT_PASSWORD: process.env.MONGO_INITDB_ROOT_PASSWORD || 'mongo',
    NO_CACHE: parseBool(process.env.NO_CACHE, false),
    COLLECTION_NAME: process.env.MONGODB_COLLECTION || 'knightcrawler_consumer_collection'
}

// Combine the environment variables into a connection string
// The combined string will look something like:
// 'mongodb://mongo:mongo@localhost:27017/knightcrawler?authSource=admin'
cacheConfig.MONGO_URI = 'mongodb://' + cacheConfig.MONGO_INITDB_ROOT_USERNAME + ':' + cacheConfig.MONGO_INITDB_ROOT_PASSWORD + '@' + cacheConfig.MONGODB_HOST + ':' + cacheConfig.MONGODB_PORT + '/' + cacheConfig.MONGODB_DB + '?authSource=admin';

export const databaseConfig = {
    POSTGRES_HOST: process.env.POSTGRES_HOST || 'postgres',
    POSTGRES_PORT: process.env.POSTGRES_PORT || '5432',
    POSTGRES_DB: process.env.POSTGRES_DB || 'knightcrawler',
    POSTGRES_USER: process.env.POSTGRES_USER || 'postgres',
    POSTGRES_PASSWORD: process.env.POSTGRES_PASSWORD || 'postgres',
    AUTO_CREATE_AND_APPLY_MIGRATIONS: parseBool(process.env.AUTO_CREATE_AND_APPLY_MIGRATIONS, false)
}

// Combine the environment variables into a connection string
// The combined string will look something like:
// 'postgres://postgres:postgres@localhost:5432/knightcrawler'
databaseConfig.POSTGRES_URI = 'postgres://' + databaseConfig.POSTGRES_USER + ':' + databaseConfig.POSTGRES_PASSWORD + '@' + databaseConfig.POSTGRES_HOST + ':' + databaseConfig.POSTGRES_PORT + '/' + databaseConfig.POSTGRES_DB;

export const jobConfig = {
    JOB_CONCURRENCY: parseInt(process.env.JOB_CONCURRENCY || 1),
    JOBS_ENABLED: parseBool(process.env.JOBS_ENABLED || true)
}

export const metadataConfig = {
    IMDB_CONCURRENT: parseInt(process.env.IMDB_CONCURRENT || 1),
    IMDB_INTERVAL_MS: parseInt(process.env.IMDB_INTERVAL_MS || 1000),
}

export const trackerConfig = {
    TRACKERS_URL: process.env.TRACKERS_URL || 'https://ngosang.github.io/trackerslist/trackers_all.txt',
    UDP_ENABLED: parseBool(process.env.UDP_TRACKERS_ENABLED || false),
}

export const torrentConfig = {
    MAX_CONNECTIONS_PER_TORRENT: parseInt(process.env.MAX_SINGLE_TORRENT_CONNECTIONS || 20),
    TIMEOUT: parseInt(process.env.TORRENT_TIMEOUT || 30000),
}

function parseBool(boolString, defaultValue) {
    const isString = typeof boolString === 'string' || boolString instanceof String;

    if (!isString) {
        return defaultValue;
    }

    return boolString.toLowerCase() === 'true' ? true : defaultValue;
}