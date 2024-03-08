import "reflect-metadata"; // required
import {ILoggingService} from '@interfaces/logging_service';
import {CacheMethod, CacheService} from '@services/cache_service';
import {IocTypes} from "@setup/ioc_types";
import {Container} from "inversify";

jest.mock('@services/configuration_service', () => {
    return {
        configurationService: {
            cacheConfig: {
                MONGODB_HOST: 'localhost',
                MONGODB_PORT: '27017',
                MONGODB_DB: 'knightcrawler',
                MONGODB_USER: 'mongo',
                MONGODB_PASSWORD: 'mongo',
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
        jest.clearAllMocks();
        process.env.LOG_LEVEL = 'debug';
        cacheMethod = jest.fn().mockResolvedValue({});
        loggingService = jest.requireMock<ILoggingService>('@services/logging_service');
        const container = new Container();
        container.bind<CacheService>(CacheService).toSelf();
        container.bind<ILoggingService>(IocTypes.ILoggingService).toConstantValue(loggingService);
        cacheService = container.get(CacheService);
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

        const result = await cacheService.cacheWrapImdbId('testKey', cacheMethod);
        expect(result).toBeDefined();
    });
});
