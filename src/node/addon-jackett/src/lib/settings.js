const parseBool = (boolString, defaultValue)=> {
    const isString = typeof boolString === 'string' || boolString instanceof String;

    if (!isString) {
        return defaultValue;
    }

    return boolString.toLowerCase() === 'true' ? true  : defaultValue;
}

export const jackettConfig = {
    URI: process.env.JACKETT_URI,
    API_KEY: process.env.JACKETT_API_KEY,
    TIMEOUT: parseInt(process.env.JACKETT_TIMEOUT || 10000),
    MAXIMUM_RESULTS: parseInt(process.env.JACKETT_MAXIMUM_RESULTS || 20),
}

export const processConfig = {
    DEBUG: parseBool(process.env.DEBUG_MODE, false),
    PORT: parseInt(process.env.PORT || 7001),
}

export const cinemetaConfig = {
    URI: process.env.CINEMETA_URI || 'https://v3-cinemeta.strem.io/meta',
}

export const cacheConfig = {
    MONGODB_URI: process.env.MONGODB_URI,
    NO_CACHE: parseBool(process.env.NO_CACHE, false),
    IMDB_TTL: parseInt(process.env.IMDB_TTL || 60 * 60 * 4), // 4 Hours
    STREAM_TTL: parseInt(process.env.STREAM_TTL || 60 * 60 * 4), // 1 Hour
    STREAM_EMPTY_TTL: parseInt(process.env.STREAM_EMPTY_TTL || 60), // 60 seconds
    AVAILABILITY_TTL: parseInt(process.env.AVAILABILITY_TTL || 8 * 60 * 60), // 8 hours
    AVAILABILITY_EMPTY_TTL: parseInt(process.env.AVAILABILITY_EMPTY_TTL || 30 * 60), // 30 minutes
    MESSAGE_VIDEO_URL_TTL: parseInt(process.env.MESSAGE_VIDEO_URL_TTL || 60), // 1 minutes
    CACHE_MAX_AGE: parseInt(process.env.CACHE_MAX_AGE) || 60 * 60, // 1 hour in seconds
    CACHE_MAX_AGE_EMPTY: parseInt(process.env.CACHE_MAX_AGE_EMPTY) || 60, // 60 seconds
    CATALOG_CACHE_MAX_AGE: parseInt(process.env.CATALOG_CACHE_MAX_AGE) || 20 * 60, // 20 minutes
    STALE_REVALIDATE_AGE: parseInt(process.env.STALE_REVALIDATE_AGE) || 4 * 60 * 60, // 4 hours
    STALE_ERROR_AGE: parseInt(process.env.STALE_ERROR_AGE) || 7 * 24 * 60 * 60, // 7 days
    GLOBAL_KEY_PREFIX: process.env.GLOBAL_KEY_PREFIX || 'jackettio-addon',
}