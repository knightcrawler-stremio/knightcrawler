import "reflect-metadata"; // required
import {TorrentType} from "@enums/torrent_types";
import {ILoggingService} from "@interfaces/logging_service";
import {MongoRepository} from "@mongo/mongo_repository";
import {IocTypes} from "@setup/ioc_types";
import {Container, inject} from "inversify";

jest.mock('@services/configuration_service', () => {
    return {
        configurationService: {
            cacheConfig: {
                MONGODB_HOST: 'localhost',
                MONGODB_PORT: '27017',
                MONGODB_DB: 'knightcrawler',
                MONGODB_USER: 'mongo',
                MONGODB_PASSWORD: 'mongo',
                get MONGO_URI(): string {
                    return `mongodb://${this.MONGODB_USER}:${this.MONGODB_PASSWORD}@${this.MONGODB_HOST}:${this.MONGODB_PORT}/${this.MONGODB_DB}?authSource=admin`;
                }
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

xdescribe('MongoRepository Tests - Manual Tests against real cluster. Skipped by default.', () => {
    let mongoRepository: MongoRepository,
        mockLogger: ILoggingService;

    beforeEach(() => {
        jest.clearAllMocks();
        process.env.LOG_LEVEL = 'debug';
        mockLogger = jest.requireMock<ILoggingService>('@services/logging_service');
        const container = new Container();
        container.bind<ILoggingService>(IocTypes.ILoggingService).toConstantValue(mockLogger);
        container.bind<MongoRepository>(MongoRepository).toSelf();
        mongoRepository = container.get(MongoRepository);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should get The Flash 2014 imdbId correctly', async () => {
        await mongoRepository.connect();
        const result = await mongoRepository.getImdbId('The Flash', TorrentType.Series, 2014);
        expect(result).toBe('tt3107288');
    });

    it('should get The Flash 1990 imdbId correctly', async () => {
        await mongoRepository.connect();
        const result = await mongoRepository.getImdbId('The Flash', TorrentType.Series, 1990);
        expect(result).toBe('tt0098798');
    });

    it('should get Wrath of Khan imdbId correctly', async () => {
        await mongoRepository.connect();
        const result = await mongoRepository.getImdbId('Star Trek II: The Wrath of Khan', TorrentType.Movie, 1982);
        expect(result).toBe('tt0084726');
    }, 30000);

    it('should get Wrath of Khan simple imdbId correctly', async () => {
        await mongoRepository.connect();
        const result = await mongoRepository.getImdbId('Wrath of Khan', TorrentType.Movie, 1982);
        expect(result).toBe('tt0084726');
    }, 30000);

    it('should get Tom and Jerry imdbId correctly', async () => {
        await mongoRepository.connect();
        const result = await mongoRepository.getImdbId('Tom and Jerry Tales', TorrentType.Series);
        expect(result).toBe('tt0780438');
    }, 30000);

    it('should get Return of the Jedi correctly', async () => {
        await mongoRepository.connect();
        const result = await mongoRepository.getImdbId('Star Wars: Episode VI - Return of the Jedi', TorrentType.Movie, 1983);
        expect(result).toBe('tt0086190');
    }, 30000);

    it('should get Return of the Jedi correctly', async () => {
        await mongoRepository.connect();
        const result = await mongoRepository.getImdbId('Return of the Jedi', TorrentType.Movie, 1983);
        expect(result).toBe('tt0086190');
    }, 30000);
});
