import {CacheMethod} from "../services/cache_service";

/* eslint-disable @typescript-eslint/no-explicit-any */
export interface ICacheService {
    cacheWrapImdbId: (key: string, method: CacheMethod) => Promise<any>;
    cacheWrapKitsuId: (key: string, method: CacheMethod) => Promise<any>;
    cacheWrapMetadata: (id: string, method: CacheMethod) => Promise<any>;
    cacheTrackers: (method: CacheMethod) => Promise<any>;
}
/* eslint-enable @typescript-eslint/no-explicit-any */