import "reflect-metadata"; // required
import { ILoggingService } from '@interfaces/logging_service';
import { CacheService, CacheMethod } from '@services/cache_service';

jest.mock('@services/configuration_service', () => {
    return {
        configurationService: {
            cacheConfig: {
                MONGODB_HOST: 'localhost',
                MONGODB_PORT: '27017',
                MONGODB_DB: 'knightcrawler',
                MONGO_INITDB_ROOT_USERNAME: 'mongo',
                MONGO_INITDB_ROOT_PASSWORD: 'mongo',
                NO_CACHE: false,
                COLLECTION_NAME: 'knightcrawler_consumer_collection',                
            },
        }
    }
});

jest.mock('@services/logging_service', () => {
    return {
        error: jest.fn(),
        info: jest.fn(),
        debug: jest.fn(),
    }
})

jest.mock('cache-manager', () => {
    return {
        createCache: jest.fn().mockReturnValue({
            wrap: jest.fn().mockImplementation((_, method) => method()),
        }),
        memoryStore: jest.fn(),
    };
});

jest.mock('@tirke/node-cache-manager-mongodb', () => {
    return {
        mongoDbStore: jest.fn(),
    };
});

describe('CacheService Tests', () => {
    let cacheService: CacheService,
     loggingService: ILoggingService,
     cacheMethod: CacheMethod;

    beforeEach(() => {
        process.env.LOG_LEVEL = 'debug';
        loggingService = jest.requireMock<ILoggingService>('@services/logging_service');
        cacheMethod = jest.fn().mockResolvedValue({});
        cacheService = new CacheService(loggingService);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should cacheWrapImdbId correctly', async () => {
        const result = await cacheService.cacheWrapImdbId('testKey', cacheMethod);
        expect(result).toBeDefined();
        expect(loggingService.debug).toHaveBeenCalledTimes(3);
    });

    it('should cacheWrapKitsuId correctly', async () => {
        const result = await cacheService.cacheWrapKitsuId('testKey', cacheMethod);
        expect(result).toBeDefined();
        expect(loggingService.debug).toHaveBeenCalledTimes(3);
    });

    it('should cacheWrapMetadata correctly', async () => {
        const result = await cacheService.cacheWrapMetadata('testId', cacheMethod);
        expect(result).toBeDefined();
        expect(loggingService.debug).toHaveBeenCalledTimes(3);
    });

    it('should cacheTrackers correctly', async () => {
        const result = await cacheService.cacheTrackers(cacheMethod);
        expect(result).toBeDefined();
        expect(loggingService.debug).toHaveBeenCalledTimes(3);
    });

    it('should handle error in cacheMethod for cacheWrapImdbId', async () => {
        cacheMethod = jest.fn().mockRejectedValue(new Error('Test error'));
        await expect(cacheService.cacheWrapImdbId('testKey', cacheMethod)).rejects.toThrow('Test error');
    });

    it('should handle error in cacheMethod for cacheWrapKitsuId', async () => {
        cacheMethod = jest.fn().mockRejectedValue(new Error('Test error'));
        await expect(cacheService.cacheWrapKitsuId('testKey', cacheMethod)).rejects.toThrow('Test error');
    });

    it('should handle error in cacheMethod for cacheWrapMetadata', async () => {
        cacheMethod = jest.fn().mockRejectedValue(new Error('Test error'));
        await expect(cacheService.cacheWrapMetadata('testId', cacheMethod)).rejects.toThrow('Test error');
    });

    it('should handle error in cacheMethod for cacheTrackers', async () => {
        cacheMethod = jest.fn().mockRejectedValue(new Error('Test error'));
        await expect(cacheService.cacheTrackers(cacheMethod)).rejects.toThrow('Test error');
    });
    
    it('should handle when cache is disabled', async () => {
        jest.mock('@services/configuration_service', () => {
            return {
                configurationService: {
                    cacheConfig: {
                        NO_CACHE: true,
                    },
                }
            }
        });

        cacheService = new CacheService(loggingService);
        const result = await cacheService.cacheWrapImdbId('testKey', cacheMethod);
        expect(result).toBeDefined();
    });
});