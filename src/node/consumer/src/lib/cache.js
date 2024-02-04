import { createCache, memoryStore} from 'cache-manager';
import { mongoDbStore } from '@tirke/node-cache-manager-mongodb'
import { cacheConfig } from './config.js';
import { logger } from './logger.js';
import { CacheType } from "./types.js";

const GLOBAL_KEY_PREFIX = 'selfhostio-consumer';
const IMDB_ID_PREFIX = `${GLOBAL_KEY_PREFIX}|imdb_id`;
const KITSU_ID_PREFIX = `${GLOBAL_KEY_PREFIX}|kitsu_id`;
const METADATA_PREFIX = `${GLOBAL_KEY_PREFIX}|metadata`;
const TRACKERS_KEY_PREFIX = `${GLOBAL_KEY_PREFIX}|trackers`;

const GLOBAL_TTL = process.env.METADATA_TTL || 7 * 24 * 60 * 60; // 7 days
const MEMORY_TTL = process.env.METADATA_TTL || 2 * 60 * 60; // 2 hours
const TRACKERS_TTL = 2 * 24 * 60 * 60; // 2 days

const initiateMemoryCache = () =>
    createCache(memoryStore(), {
        ttl: parseInt(MEMORY_TTL)
    }); 

const initiateMongoCache = () => {
    const store = mongoDbStore({
        collectionName: cacheConfig.COLLECTION_NAME,
        ttl: parseInt(GLOBAL_TTL),
        url: cacheConfig.MONGO_URI,
        mongoConfig:{
            socketTimeoutMS: 120000,
            appName: 'selfhostio-consumer',
        }
    });    
    
    return createCache(store, {
        ttl: parseInt(GLOBAL_TTL),
    });
}

const initiateRemoteCache = ()=> {
    if (cacheConfig.NO_CACHE) {
        logger.debug('Cache is disabled');
        return null;
    }
    return cacheConfig.MONGO_URI ? initiateMongoCache() : initiateMemoryCache();
}

const getCacheType = (cacheType) => {
    switch (cacheType) {
        case CacheType.MEMORY:
            return memoryCache;
        case CacheType.MONGODB:
            return remoteCache;
        default:
            return null;
    }
}

const memoryCache = initiateMemoryCache()
const remoteCache = initiateRemoteCache()

const cacheWrap = async (cacheType, key, method, options) => {
    const cache = getCacheType(cacheType);
    
    if (cacheConfig.NO_CACHE || !cache) {
        return method();
    }

    logger.debug(`Cache type: ${cacheType}`);
    logger.debug(`Cache key: ${key}`);
    logger.debug(`Cache options: ${JSON.stringify(options)}`);
        
    return cache.wrap(key, method, options.ttl);
}

export const cacheWrapImdbId = (key, method) => 
    cacheWrap(CacheType.MONGODB, `${IMDB_ID_PREFIX}:${key}`, method, { ttl: parseInt(GLOBAL_TTL) });

export const cacheWrapKitsuId = (key, method) => 
    cacheWrap(CacheType.MONGODB, `${KITSU_ID_PREFIX}:${key}`, method, { ttl: parseInt(GLOBAL_TTL) });

export const cacheWrapMetadata = (id, method) => 
    cacheWrap(CacheType.MEMORY, `${METADATA_PREFIX}:${id}`, method, { ttl: parseInt(MEMORY_TTL) });

export const cacheTrackers = (method) => 
    cacheWrap(CacheType.MEMORY, `${TRACKERS_KEY_PREFIX}`, method, { ttl: parseInt(TRACKERS_TTL) });