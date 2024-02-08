import "reflect-metadata"; // required

describe('Configuration Tests', () => {
    let configurationService;
    beforeAll(async () => {
        process.env.MONGODB_HOST = 'test_mongodb';
        process.env.MONGODB_PORT = '27017';
        process.env.MONGODB_DB = 'knightcrawler';
        process.env.MONGO_INITDB_ROOT_USERNAME = 'mongo';
        process.env.MONGO_INITDB_ROOT_PASSWORD = 'mongo';
        process.env.NO_CACHE = 'false';
        process.env.MONGODB_COLLECTION = 'knightcrawler_consumer_collection';
        process.env.POSTGRES_HOST = 'postgres';
        process.env.POSTGRES_PORT = '5432';
        process.env.POSTGRES_DB = 'knightcrawler';
        process.env.POSTGRES_USER = 'postgres';
        process.env.POSTGRES_PASSWORD = 'postgres';
        process.env.AUTO_CREATE_AND_APPLY_MIGRATIONS = 'false';
        process.env.TRACKERS_URL = 'https://ngosang.github.io/trackerslist/trackers_all.txt';
        process.env.UDP_TRACKERS_ENABLED = 'false';
        process.env.MAX_CONNECTIONS_PER_TORRENT = '20';
        process.env.TORRENT_TIMEOUT = '30000';
        process.env.RABBIT_URI = 'amqp://localhost';
        process.env.QUEUE_NAME = 'test-queue';
        process.env.IMDB_CONCURRENT = '1';
        process.env.IMDB_INTERVAL_MS = '1000';
        process.env.JOB_CONCURRENCY = '1';
        process.env.JOBS_ENABLED = 'true';
        
        // shitty hack cause jest caches modules and resetModules isnt working
        ({ configurationService } = await import("@services/configuration_service")); 
    });
    
    it('should populate cacheConfig correctly', () => {
        const { cacheConfig } = configurationService;
        expect(cacheConfig.MONGODB_HOST).toBe('test_mongodb');
        expect(cacheConfig.MONGODB_PORT).toBe('27017');
        expect(cacheConfig.MONGODB_DB).toBe('knightcrawler');
        expect(cacheConfig.MONGO_INITDB_ROOT_USERNAME).toBe('mongo');
        expect(cacheConfig.MONGO_INITDB_ROOT_PASSWORD).toBe('mongo');
        expect(cacheConfig.NO_CACHE).toBe(false);
        expect(cacheConfig.COLLECTION_NAME).toBe('knightcrawler_consumer_collection');
        expect(cacheConfig.MONGO_URI).toBe('mongodb://mongo:mongo@test_mongodb:27017/knightcrawler?authSource=admin');
    });

    it('should populate databaseConfig correctly', () => {
        const { databaseConfig } = configurationService;
        expect(databaseConfig.POSTGRES_HOST).toBe('postgres');
        expect(databaseConfig.POSTGRES_PORT).toBe(5432);
        expect(databaseConfig.POSTGRES_DB).toBe('knightcrawler');
        expect(databaseConfig.POSTGRES_USER).toBe('postgres');
        expect(databaseConfig.POSTGRES_PASSWORD).toBe('postgres');
        expect(databaseConfig.AUTO_CREATE_AND_APPLY_MIGRATIONS).toBe(false);
        expect(databaseConfig.POSTGRES_URI).toBe('postgres://postgres:postgres@postgres:5432/knightcrawler');
    });

    it('should populate jobConfig correctly', () => {
        const { jobConfig } = configurationService;
        expect(jobConfig.JOB_CONCURRENCY).toBe(1);
        expect(jobConfig.JOBS_ENABLED).toBe(true);
    });

    it('should populate metadataConfig correctly', () => {
        const { metadataConfig } = configurationService;
        expect(metadataConfig.IMDB_CONCURRENT).toBe(1);
        expect(metadataConfig.IMDB_INTERVAL_MS).toBe(1000);
    });

    it('should populate rabbitConfig correctly', () => {
        const { rabbitConfig } = configurationService;
        expect(rabbitConfig.RABBIT_URI).toBe('amqp://localhost');
        expect(rabbitConfig.QUEUE_NAME).toBe('test-queue');
    });

    it('should populate torrentConfig correctly', () => {
        const { torrentConfig } = configurationService;
        expect(torrentConfig.MAX_CONNECTIONS_PER_TORRENT).toBe(20);
        expect(torrentConfig.TIMEOUT).toBe(30000);
    });

    it('should populate trackerConfig correctly', () => {
        const { trackerConfig } = configurationService;
        expect(trackerConfig.TRACKERS_URL).toBe('https://ngosang.github.io/trackerslist/trackers_all.txt');
        expect(trackerConfig.UDP_ENABLED).toBe(false);
    });
    
});