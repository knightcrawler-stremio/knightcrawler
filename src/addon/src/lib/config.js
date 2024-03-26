export const cacheConfig = {
    REDIS_HOST: process.env.REDIS_HOST || 'redis',
    REDIS_PORT: process.env.REDIS_PORT || '6379',
    REDIS_EXTRA: process.env.REDIS_EXTRA || '',
    NO_CACHE: parseBool(process.env.NO_CACHE, false),
}

cacheConfig.REDIS_CONNECTION_STRING = 'redis://' + cacheConfig.REDIS_HOST + ':' + cacheConfig.REDIS_PORT + '?' + cacheConfig.REDIS_EXTRA;

export const databaseConfig = {
    POSTGRES_HOST: process.env.POSTGRES_HOST || 'postgres',
    POSTGRES_PORT: process.env.POSTGRES_PORT || '5432',
    POSTGRES_DB: process.env.POSTGRES_DB || 'knightcrawler',
    POSTGRES_USER: process.env.POSTGRES_USER || 'postgres',
    POSTGRES_PASSWORD: process.env.POSTGRES_PASSWORD || 'postgres',
}

// Combine the environment variables into a connection string
// The combined string will look something like:
// 'postgres://postgres:postgres@localhost:5432/knightcrawler'
databaseConfig.POSTGRES_URI = 'postgres://' + databaseConfig.POSTGRES_USER + ':' + databaseConfig.POSTGRES_PASSWORD + '@' + databaseConfig.POSTGRES_HOST + ':' + databaseConfig.POSTGRES_PORT + '/' + databaseConfig.POSTGRES_DB;


function parseBool(boolString, defaultValue) {
    const isString = typeof boolString === 'string' || boolString instanceof String;

    if (!isString) {
        return defaultValue;
    }

    return boolString.toLowerCase() === 'true' ? true : defaultValue;
}
