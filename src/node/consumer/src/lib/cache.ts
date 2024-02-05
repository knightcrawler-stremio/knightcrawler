import {Cache, createCache, memoryStore} from 'cache-manager';
import { mongoDbStore } from '@tirke/node-cache-manager-mongodb'
import { cacheConfig } from './config';
import { logger } from './logger';
import { CacheType } from "./enums/cache_types";
import {CacheOptions} from "./interfaces/cache_options";

const GLOBAL_KEY_PREFIX = 'knightcrawler-consumer';
const IMDB_ID_PREFIX = `${GLOBAL_KEY_PREFIX}|imdb_id`;
const KITSU_ID_PREFIX = `${GLOBAL_KEY_PREFIX}|kitsu_id`;
const METADATA_PREFIX = `${GLOBAL_KEY_PREFIX}|metadata`;
const TRACKERS_KEY_PREFIX = `${GLOBAL_KEY_PREFIX}|trackers`;

const GLOBAL_TTL: number = Number(process.env.METADATA_TTL) || 7 * 24 * 60 * 60; // 7 days
const MEMORY_TTL: number = Number(process.env.METADATA_TTL) || 2 * 60 * 60; // 2 hours
const TRACKERS_TTL: number = 2 * 24 * 60 * 60; // 2 days

type CacheMethod = () => any;

const initiateMemoryCache = () =>
    createCache(memoryStore(), {
        ttl: MEMORY_TTL
    }) as Cache;

const initiateMongoCache = () => {
    const store = mongoDbStore({
        collectionName: cacheConfig.COLLECTION_NAME,
        ttl: GLOBAL_TTL,
        url: cacheConfig.MONGO_URI,
        mongoConfig:{
            socketTimeoutMS: 120000,
            appName: 'knightcrawler-consumer',
        }
    });

    return createCache(store, {
        ttl: GLOBAL_TTL,
    });
}

const initiateRemoteCache = (): Cache => {
    if (cacheConfig.NO_CACHE) {
        logger.debug('Cache is disabled');
        return null;
    }
    
    return cacheConfig.MONGO_URI ? initiateMongoCache() : initiateMemoryCache();
}

const getCacheType = (cacheType: CacheType): typeof memoryCache | null => {
    switch (cacheType) {
        case CacheType.MEMORY:
            return memoryCache;
        case CacheType.MONGODB:
            return remoteCache;
        default:
            return null;
    }
}

const memoryCache = initiateMemoryCache();
const remoteCache = initiateRemoteCache();

const cacheWrap = async (
    cacheType: CacheType, key: string, method: CacheMethod, options: CacheOptions): Promise<any> => {
    const cache = getCacheType(cacheType);

    if (cacheConfig.NO_CACHE || !cache) {
        return method();
    }

    logger.debug(`Cache type: ${cacheType}`);
    logger.debug(`Cache key: ${key}`);
    logger.debug(`Cache options: ${JSON.stringify(options)}`);

    return cache.wrap(key, method, options.ttl);
}

export const cacheWrapImdbId = (key: string, method: CacheMethod): Promise<any> =>
    cacheWrap(CacheType.MONGODB, `${IMDB_ID_PREFIX}:${key}`, method, { ttl: GLOBAL_TTL });

export const cacheWrapKitsuId = (key: string, method: CacheMethod): Promise<any> =>
    cacheWrap(CacheType.MONGODB, `${KITSU_ID_PREFIX}:${key}`, method, { ttl: GLOBAL_TTL });

export const cacheWrapMetadata = (id: string, method: CacheMethod): Promise<any> =>
    cacheWrap(CacheType.MEMORY, `${METADATA_PREFIX}:${id}`, method, { ttl: MEMORY_TTL });

export const cacheTrackers = (method: CacheMethod): Promise<any> =>
    cacheWrap(CacheType.MEMORY, `${TRACKERS_KEY_PREFIX}`, method, { ttl: TRACKERS_TTL });