import "reflect-metadata"; // required
import {TorrentType} from "@enums/torrent_types";
import {MongoRepository} from "@mongo/mongo_repository";
import {Container} from "inversify";

jest.mock('@services/configuration_service', () => {
    return {
        configurationService: {
            cacheConfig: {
                MONGODB_HOST: 'localhost',
                MONGODB_PORT: '27017',
                MONGODB_DB: 'knightcrawler',
                MONGO_INITDB_ROOT_USERNAME: 'mongo',
                MONGO_INITDB_ROOT_PASSWORD: 'mongo',
                get MONGO_URI(): string {
                    return `mongodb://${this.MONGO_INITDB_ROOT_USERNAME}:${this.MONGO_INITDB_ROOT_PASSWORD}@${this.MONGODB_HOST}:${this.MONGODB_PORT}/${this.MONGODB_DB}?authSource=admin`;
                }
            },
        }
    }
});

describe('MongoRepository Tests', () => {
    let mongoRepository: MongoRepository;
        
    beforeEach(() => {
        jest.clearAllMocks();
        process.env.LOG_LEVEL = 'debug';
        const container = new Container();
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

    it('should get Wrath of Khan imdbId correctly', async () => {
        await mongoRepository.connect();
        const result = await mongoRepository.getImdbId('Wrath of Khan', TorrentType.Movie, 1982);
        expect(result).toBe('tt0084726');
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