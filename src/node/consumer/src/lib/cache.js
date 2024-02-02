import cacheManager from 'cache-manager';
import mangodbStore from 'cache-manager-mongodb';
import { cacheConfig } from './config.js';

const GLOBAL_KEY_PREFIX = 'selfhostio-consumer';
const IMDB_ID_PREFIX = `${GLOBAL_KEY_PREFIX}|imdb_id`;
const KITSU_ID_PREFIX = `${GLOBAL_KEY_PREFIX}|kitsu_id`;
const METADATA_PREFIX = `${GLOBAL_KEY_PREFIX}|metadata`;
const TRACKERS_KEY_PREFIX = `${GLOBAL_KEY_PREFIX}|trackers`;

const GLOBAL_TTL = process.env.METADATA_TTL || 7 * 24 * 60 * 60; // 7 days
const MEMORY_TTL = process.env.METADATA_TTL || 2 * 60 * 60; // 2 hours
const TRACKERS_TTL = 2 * 24 * 60 * 60; // 2 days

const memoryCache = initiateMemoryCache();
const remoteCache = initiateRemoteCache();

function initiateRemoteCache() {
    if (cacheConfig.NO_CACHE) {
        return null;
    } else if (cacheConfig.MONGO_URI) {
        return cacheManager.caching({
            store: mangodbStore,
            uri: cacheConfig.MONGO_URI,
            options: {
                collection: cacheConfig.COLLECTION_NAME,
                socketTimeoutMS: 120000,
                useNewUrlParser: true,
                useUnifiedTopology: false,
                ttl: GLOBAL_TTL
            },
            ttl: GLOBAL_TTL,
            ignoreCacheErrors: true
        });
    } else {
        return cacheManager.caching({
            store: 'memory',
            ttl: MEMORY_TTL
        });
    }
}

function initiateMemoryCache() {
    return cacheManager.caching({
        store: 'memory',
        ttl: MEMORY_TTL,
        max: Infinity // infinite LRU cache size
    });
}

function cacheWrap(cache, key, method, options) {
    if (cacheConfig.NO_CACHE || !cache) {
        return method();
    }
    return cache.wrap(key, method, options);
}

export function cacheWrapImdbId(key, method) {
    return cacheWrap(remoteCache, `${IMDB_ID_PREFIX}:${key}`, method, { ttl: GLOBAL_TTL });
}

export function cacheWrapKitsuId(key, method) {
    return cacheWrap(remoteCache, `${KITSU_ID_PREFIX}:${key}`, method, { ttl: GLOBAL_TTL });
}

export function cacheWrapMetadata(id, method) {
    return cacheWrap(memoryCache, `${METADATA_PREFIX}:${id}`, method, { ttl: MEMORY_TTL });
}

export function cacheTrackers(method) {
    return cacheWrap(memoryCache, `${TRACKERS_KEY_PREFIX}`, method, { ttl: TRACKERS_TTL });
}