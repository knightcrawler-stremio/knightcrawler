import Bottleneck from 'bottleneck';
import {addonBuilder} from 'stremio-addon-sdk';
import {searchJackett} from "./jackett/jackett.js";
import {cacheWrapStream} from './lib/cache.js';
import {getMetaData} from "./lib/cinemetaProvider.js";
import {dummyManifest} from './lib/manifest.js';
import {cacheConfig, processConfig} from "./lib/settings.js";
import applySorting from './lib/sort.js';
import {toStreamInfo} from './lib/streamInfo.js';
import {Type} from './lib/types.js';
import {applyMochs, getMochCatalog, getMochItemMeta} from './moch/moch.js';
import StaticLinks from './moch/static.js';

const builder = new addonBuilder(dummyManifest());
const limiter = new Bottleneck({
  maxConcurrent: 200,
  highWater: 220,
  strategy: Bottleneck.strategy.OVERFLOW
});

builder.defineStreamHandler((args) => {
  if (!args.id.match(/tt\d+/i) && !args.id.match(/kitsu:\d+/i)) {
    return Promise.resolve({ streams: [] });
  }
  
  if (processConfig.DEBUG) {
    console.log(`Incoming stream ${args.id} request`)
    console.log('args', args);
  }
  
  return cacheWrapStream(args.id, () => limiter.schedule(() =>
      streamHandler(args)
      .then(records => records.map(record => toStreamInfo(record, args.type))))
      .then(streams => applySorting(streams, args.extra))
      .then(streams => applyMochs(streams, args.extra))
      .then(streams => enrichCacheParams(streams))
      .catch(error => {
        console.log(`Failed request ${args.id}: ${error}`);
        return Promise.reject(`Failed request ${args.id}: ${error}`);
      }));
});


builder.defineCatalogHandler((args) => {
  const mochKey = args.id.replace("jackettio-", '');
  console.log(`Incoming catalog ${args.id} request with skip=${args.extra.skip || 0}`)
  return getMochCatalog(mochKey, args.extra)
      .then(metas => ({
        metas: metas,
        cacheMaxAge: cacheConfig.CATALOG_CACHE_MAX_AGE
      }))
      .catch(error => {
        return Promise.reject(`Failed retrieving catalog ${args.id}: ${JSON.stringify(error)}`);
      });
})

builder.defineMetaHandler((args) => {
  const [mochKey, metaId] = args.id.split(':');
  console.log(`Incoming debrid meta ${args.id} request`)
  return getMochItemMeta(mochKey, metaId, args.extra)
      .then(meta => ({
        meta: meta,
        cacheMaxAge: metaId === 'Downloads' ? 0 : cacheConfig.CACHE_MAX_AGE
      }))
      .catch(error => {
        return Promise.reject(`Failed retrieving catalog meta ${args.id}: ${JSON.stringify(error)}`);
      });
})

async function streamHandler(args) {
  if (args.type === Type.MOVIE) {
    return movieRecordsHandler(args);
  } else if (args.type === Type.SERIES) {
    return seriesRecordsHandler(args);
  }
  return Promise.reject('not supported type');
}

async function seriesRecordsHandler(args) {
    if (args.id.match(/^tt\d+:\d+:\d+$/)) {
        const parts = args.id.split(':');
        const season = parts[1] !== undefined ? parseInt(parts[1], 10) : 1;
        const episode = parts[2] !== undefined ? parseInt(parts[2], 10) : 1;

        const metaData = await getMetaData(args);
        return await searchJackett({
            type: Type.SERIES,
            season: season,
            episode: episode,
            name: metaData.name,
        });
    }

    return [];
}

async function movieRecordsHandler(args) {
  if (args.id.match(/^tt\d+$/)) {

      const metaData = await getMetaData(args);
      return await searchJackett({
          type: Type.MOVIE,
          name: metaData.name,
          year: metaData.year,
      });
  }

    return [];
}

function enrichCacheParams(streams) {
  let cacheAge = cacheConfig.CACHE_MAX_AGE;
  if (!streams.length) {
    cacheAge = cacheConfig.CACHE_MAX_AGE_EMPTY;
  } else if (streams.every(stream => stream?.url?.endsWith(StaticLinks.FAILED_ACCESS))) {
    cacheAge = 0;
  }
  return {
    streams: streams,
    cacheMaxAge: cacheAge,
    staleRevalidate: cacheConfig.STALE_REVALIDATE_AGE,
    staleError: cacheConfig.STALE_ERROR_AGE
  }
}

export default builder.getInterface();
