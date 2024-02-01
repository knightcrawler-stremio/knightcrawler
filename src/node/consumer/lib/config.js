export const rabbitConfig = {
    URI: process.env.RABBIT_URI || 'amqp://localhost',
    QUEUE_NAME: process.env.QUEUE_NAME || 'test-queue'
}

export const cacheConfig = {
    MONGO_URI: process.env.MONGODB_URI || 'mongodb://mongo:mongo@localhost:27017/selfhostio?authSource=admin',
    NO_CACHE: parseBool(process.env.NO_CACHE, false),
    COLLECTION_NAME: process.env.MONGODB_COLLECTION || 'selfhostio_consumer_collection'
}

export const databaseConfig = {
    DATABASE_URI: process.env.POSTGRES_DATABASE_URI || 'postgres://postgres:postgres@localhost:5432/selfhostio',
    ENABLE_SYNC: parseBool(process.env.ENABLE_SYNC, true)
}

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
     
    return boolString.toLowerCase() === 'true' ? true  : defaultValue;
}