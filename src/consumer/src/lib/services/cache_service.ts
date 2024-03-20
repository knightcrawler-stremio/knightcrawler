import {CacheType} from "@enums/cache_types";
import {ICacheOptions} from "@interfaces/cache_options";
import {ICacheService} from "@interfaces/cache_service";
import {ILoggingService} from "@interfaces/logging_service";
import {configurationService} from '@services/configuration_service';
import {IocTypes} from "@setup/ioc_types";
import {Cache, createCache, MemoryCache, memoryStore} from 'cache-manager';
import {inject, injectable} from "inversify";
import {redisStore} from "cache-manager-redis-yet";
import * as process from "process";

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
    @inject(IocTypes.ILoggingService) private logger: ILoggingService;

    private memoryCache: MemoryCache | undefined;
    private remoteCache: Cache | MemoryCache | undefined;
    private initializer: Promise<void>;

    constructor() {
        this.initializer = this.initialize();
    }

    cacheWrapImdbId(key: string, method: CacheMethod): Promise<CacheMethod> {
        return this.cacheWrap(CacheType.Redis, `${IMDB_ID_PREFIX}:${key}`, method, {ttl: GLOBAL_TTL});
    }

    cacheWrapKitsuId(key: string, method: CacheMethod): Promise<CacheMethod> {
        return this.cacheWrap(CacheType.Redis, `${KITSU_ID_PREFIX}:${key}`, method, {ttl: GLOBAL_TTL});
    }

    cacheWrapMetadata(id: string, method: CacheMethod): Promise<CacheMethod> {
        return this.cacheWrap(CacheType.Memory, `${METADATA_PREFIX}:${id}`, method, {ttl: MEMORY_TTL});
    }

    cacheTrackers(method: CacheMethod): Promise<CacheMethod> {
        return this.cacheWrap(CacheType.Redis, `${TRACKERS_KEY_PREFIX}`, method, {ttl: TRACKERS_TTL});
    }

    private initiateMemoryCache = (): MemoryCache =>
        createCache(memoryStore(), {
            ttl: MEMORY_TTL
        });

    private initiateRedisCache = async (): Promise<Cache> => {
        const store = await redisStore({
            ttl: GLOBAL_TTL,
            url: configurationService.redisConfig.CONNECTION_STRING
        });

        return createCache(store, {
            ttl: GLOBAL_TTL,
        });
    }

    private initiateRemoteCache = async (): Promise<Cache | undefined> => {
        if (configurationService.cacheConfig.NO_CACHE) {
            this.logger.debug('Cache is disabled');
            return undefined;
        }

        return configurationService.redisConfig.CONNECTION_STRING ? this.initiateRedisCache() : this.initiateMemoryCache();
    }

    private getCacheType = (cacheType: CacheType): MemoryCache | Cache | undefined => {
        switch (cacheType) {
            case CacheType.Memory:
                return this.memoryCache;
            case CacheType.Redis:
                return this.remoteCache;
            default:
                return undefined;
        }
    }

    private cacheWrap = async (cacheType: CacheType, key: string, method: CacheMethod, options: ICacheOptions): Promise<CacheMethod> => {
        await this.initializer;
        
        const cache = this.getCacheType(cacheType);

        if (configurationService.cacheConfig.NO_CACHE || !cache) {
            return method();
        }

        const expirationTime = new Date(Date.now() + options.ttl * 1000);

        this.logger.debug(`Cache type: ${cacheType}`);
        this.logger.debug(`Cache key: ${key}`);
        this.logger.debug(`Cache options: ${JSON.stringify(options)}`);
        this.logger.debug(`Cache item will expire at: ${expirationTime.toISOString()}`);

        // Memory Cache is Milliseconds, Mongo Cache converts to Seconds internally.
        const ttl : number = cacheType === CacheType.Memory ? options.ttl * 1000 : options.ttl;
        return cache.wrap(key, method, ttl);
    };
    
    private initialize = async () : Promise<void> => {
        if (configurationService.cacheConfig.NO_CACHE) {
            this.logger.info('Cache is disabled');
            return;
        }

        this.memoryCache = this.initiateMemoryCache();
        this.remoteCache = await this.initiateRemoteCache();
    }
}
