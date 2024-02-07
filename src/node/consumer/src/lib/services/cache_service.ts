import {CacheType} from "@enums/cache_types";
import {ICacheOptions} from "@interfaces/cache_options";
import {ICacheService} from "@interfaces/cache_service";
import {ILoggingService} from "@interfaces/logging_service";
import {IocTypes} from "@models/ioc_types";
import {configurationService} from '@services/configuration_service';
import {mongoDbStore} from '@tirke/node-cache-manager-mongodb'
import {Cache, createCache, MemoryCache, memoryStore} from 'cache-manager';
import {inject, injectable} from "inversify";

const GLOBAL_KEY_PREFIX = 'knightcrawler-consumer';
const IMDB_ID_PREFIX = `${GLOBAL_KEY_PREFIX}|imdb_id`;
const KITSU_ID_PREFIX = `${GLOBAL_KEY_PREFIX}|kitsu_id`;
const METADATA_PREFIX = `${GLOBAL_KEY_PREFIX}|metadata`;
const TRACKERS_KEY_PREFIX = `${GLOBAL_KEY_PREFIX}|trackers`;

const GLOBAL_TTL: number = Number(process.env.METADATA_TTL) || 7 * 24 * 60 * 60; // 7 days
const MEMORY_TTL: number = Number(process.env.METADATA_TTL) || 2 * 60 * 60; // 2 hours
const TRACKERS_TTL: number = 2 * 24 * 60 * 60; // 2 days

/* eslint-disable-next-line @typescript-eslint/no-explicit-any */
export type CacheMethod = () => any;

@injectable()
export class CacheService implements ICacheService {
    private logger: ILoggingService;
    private readonly memoryCache: MemoryCache | undefined;
    private readonly remoteCache: Cache | MemoryCache | undefined;

    constructor(@inject(IocTypes.ILoggingService) logger: ILoggingService) {
        this.logger = logger;
        if (configurationService.cacheConfig.NO_CACHE) {
            this.logger.info('Cache is disabled');
            return;
        }

        this.memoryCache = this.initiateMemoryCache();
        this.remoteCache = this.initiateRemoteCache();
    }

    public cacheWrapImdbId = (key: string, method: CacheMethod): Promise<CacheMethod> =>
        this.cacheWrap(CacheType.MongoDb, `${IMDB_ID_PREFIX}:${key}`, method, {ttl: GLOBAL_TTL});

    public cacheWrapKitsuId = (key: string, method: CacheMethod): Promise<CacheMethod> =>
        this.cacheWrap(CacheType.MongoDb, `${KITSU_ID_PREFIX}:${key}`, method, {ttl: GLOBAL_TTL});

    public cacheWrapMetadata = (id: string, method: CacheMethod): Promise<CacheMethod> =>
        this.cacheWrap(CacheType.Memory, `${METADATA_PREFIX}:${id}`, method, {ttl: MEMORY_TTL});

    public cacheTrackers = (method: CacheMethod): Promise<CacheMethod> =>
        this.cacheWrap(CacheType.Memory, `${TRACKERS_KEY_PREFIX}`, method, {ttl: TRACKERS_TTL});

    private initiateMemoryCache = (): MemoryCache =>
        createCache(memoryStore(), {
            ttl: MEMORY_TTL
        });

    private initiateMongoCache = (): Cache => {
        const store = mongoDbStore({
            collectionName: configurationService.cacheConfig.COLLECTION_NAME,
            ttl: GLOBAL_TTL,
            url: configurationService.cacheConfig.MONGO_URI,
            mongoConfig: {
                socketTimeoutMS: 120000,
                appName: 'knightcrawler-consumer',
            }
        });

        return createCache(store, {
            ttl: GLOBAL_TTL,
        });
    }

    private initiateRemoteCache = (): Cache | undefined => {
        if (configurationService.cacheConfig.NO_CACHE) {
            this.logger.debug('Cache is disabled');
            return undefined;
        }

        return configurationService.cacheConfig.MONGO_URI ? this.initiateMongoCache() : this.initiateMemoryCache();
    }

    private getCacheType = (cacheType: CacheType): MemoryCache | Cache | undefined => {
        switch (cacheType) {
            case CacheType.Memory:
                return this.memoryCache;
            case CacheType.MongoDb:
                return this.remoteCache;
            default:
                return undefined;
        }
    }

    private cacheWrap = async (
        cacheType: CacheType, key: string, method: CacheMethod, options: ICacheOptions): Promise<CacheMethod> => {
        const cache = this.getCacheType(cacheType);

        if (configurationService.cacheConfig.NO_CACHE || !cache) {
            return method();
        }

        this.logger.debug(`Cache type: ${cacheType}`);
        this.logger.debug(`Cache key: ${key}`);
        this.logger.debug(`Cache options: ${JSON.stringify(options)}`);

        return cache.wrap(key, method, options.ttl);
    }
}

