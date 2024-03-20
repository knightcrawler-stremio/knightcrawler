import cacheManager from 'cache-manager';
import mangodbStore from 'cache-manager-mongodb';
import { isStaticUrl }  from '../moch/static.js';
import {cacheConfig} from "./settings.js";

const STREAM_KEY_PREFIX = `${cacheConfig.GLOBAL_KEY_PREFIX}|stream`;
const IMDB_KEY_PREFIX = `${cacheConfig.GLOBAL_KEY_PREFIX}|imdb`;
const AVAILABILITY_KEY_PREFIX = `${cacheConfig.GLOBAL_KEY_PREFIX}|availability`;
const RESOLVED_URL_KEY_PREFIX = `${cacheConfig.GLOBAL_KEY_PREFIX}|resolved`;

const memoryCache = initiateMemoryCache();
const remoteCache = initiateRemoteCache();

function initiateRemoteCache() {
  if (cacheConfig.NO_CACHE) {
    return null;
  } else if (cacheConfig.MONGODB_URI) {
    return cacheManager.caching({
      store: mangodbStore,
      uri: cacheConfig.MONGODB_URI,
      options: {
        collection: 'jackettio_addon_collection',
        socketTimeoutMS: 120000,
        useNewUrlParser: true,
        useUnifiedTopology: false,
        ttl: cacheConfig.STREAM_EMPTY_TTL
      },
      ttl: cacheConfig.STREAM_EMPTY_TTL,
      ignoreCacheErrors: true
    });
  } else {
    return cacheManager.caching({
      store: 'memory',
      ttl: cacheConfig.STREAM_EMPTY_TTL
    });
  }
}

function initiateMemoryCache() {
  return cacheManager.caching({
    store: 'memory',
    ttl: cacheConfig.MESSAGE_VIDEO_URL_TTL,
    max: Infinity // infinite LRU cache size
  });
}

function cacheWrap(cache, key, method, options) {
  if (cacheConfig.NO_CACHE || !cache) {
    return method();
  }
  return cache.wrap(key, method, options);
}

export function cacheWrapStream(id, method) {
  return cacheWrap(remoteCache, `${STREAM_KEY_PREFIX}:${id}`, method, {
    ttl: (streams) => streams.length ? cacheConfig.STREAM_TTL : cacheConfig.STREAM_EMPTY_TTL
  });
}

export function cacheWrapImdbMetaData(id, method) {
  return cacheWrap(remoteCache, `${IMDB_KEY_PREFIX}:${id}`, method, {
    ttl: cacheConfig.IMDB_TTL
  });
}

export function cacheWrapResolvedUrl(id, method) {
  return cacheWrap(memoryCache, `${RESOLVED_URL_KEY_PREFIX}:${id}`, method, {
    ttl: (url) => isStaticUrl(url) ? cacheConfig.MESSAGE_VIDEO_URL_TTL : cacheConfig.STREAM_TTL
  });
}

export function cacheAvailabilityResults(results) {
  Object.keys(results)
      .forEach(infohash => {
        const key = `${AVAILABILITY_KEY_PREFIX}:${infohash}`;
        const value = results[infohash];
        const ttl = value?.length ? cacheConfig.AVAILABILITY_TTL : cacheConfig.AVAILABILITY_EMPTY_TTL;
        memoryCache.set(key, value, { ttl })
      });
  return results;
}

export function getCachedAvailabilityResults(infohashes) {
  const keys = infohashes.map(infohash => `${AVAILABILITY_KEY_PREFIX}:${infohash}`)
  return new Promise(resolve => {
    memoryCache.mget(...keys, (error, result) => {
      if (error) {
        console.log('Failed retrieve availability cache', error)
        return resolve({});
      }
      const availabilityResults = {};
      infohashes.forEach((infohash, index) => {
        if (result[index]) {
          availabilityResults[infohash] = result[index];
        }
      });
      resolve(availabilityResults);
    })
  });
}
