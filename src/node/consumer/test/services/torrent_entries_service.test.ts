import "reflect-metadata"; // required
import {TorrentType} from "@enums/torrent_types";
import {ILoggingService} from "@interfaces/logging_service";
import {IMetadataService} from "@interfaces/metadata_service";
import {IParsedTorrent} from "@interfaces/parsed_torrent";
import {ITorrentFileCollection} from "@interfaces/torrent_file_collection";
import {ITorrentFileService} from "@interfaces/torrent_file_service";
import {ITorrentSubtitleService} from "@interfaces/torrent_subtitle_service";
import {IDatabaseRepository} from "@repository/interfaces/database_repository";
import {IFileAttributes} from "@repository/interfaces/file_attributes";
import {ITorrentCreationAttributes} from "@repository/interfaces/torrent_attributes";
import {Torrent} from "@repository/models/torrent";
import {TorrentEntriesService} from "@services/torrent_entries_service";
import {IocTypes} from "@setup/ioc_types";
import {Container} from "inversify";

jest.mock('@services/logging_service', () => {
    return {
        error: jest.fn(),
        info: jest.fn(),
        debug: jest.fn(),
        warn: jest.fn(),
    }
})

jest.mock('@services/torrent_file_service', () => {
    return {
        parseTorrentFiles: jest.fn().mockResolvedValue(undefined),
        isPackTorrent: jest.fn().mockResolvedValue(undefined),
    }
})

jest.mock('@services/metadata_service', () => {
    return {
        getImdbId: jest.fn().mockResolvedValue(undefined),
        getKitsuId: jest.fn().mockResolvedValue(undefined),
    }
})

jest.mock('@services/torrent_subtitle_service', () => {
    return {
        assignSubtitles: jest.fn().mockResolvedValue(undefined),
    }
})

jest.mock('@repository/database_repository', () => {
    return {
        createTorrent: jest.fn().mockResolvedValue(undefined),
        createFile: jest.fn().mockResolvedValue(undefined),
        createSkipTorrent: jest.fn().mockResolvedValue(undefined),
        getSkipTorrent: jest.fn().mockResolvedValue(undefined),
        getTorrent: jest.fn().mockResolvedValue(undefined),
        setTorrentSeeders: jest.fn().mockResolvedValue(undefined),
        getFiles: jest.fn().mockResolvedValue(undefined),
    }
})

describe('TorrentEntriesService Tests', () => {
    let torrentEntriesService: TorrentEntriesService,
        mockLoggingService: ILoggingService,
        mockFileService: ITorrentFileService,
        mockMetadataService: IMetadataService,
        mockSubtitleService: ITorrentSubtitleService,
        mockDatabaseRepository: IDatabaseRepository;

    beforeEach(() => {
        jest.clearAllMocks();

        mockFileService = jest.requireMock<ITorrentFileService>('@services/torrent_file_service');
        mockMetadataService = jest.requireMock<IMetadataService>('@services/metadata_service');
        mockSubtitleService = jest.requireMock<ITorrentSubtitleService>('@services/torrent_subtitle_service');
        mockLoggingService = jest.requireMock<ILoggingService>('@services/logging_service');
        mockDatabaseRepository = jest.requireMock<IDatabaseRepository>('@repository/database_repository');

        const container = new Container();
        container.bind<TorrentEntriesService>(TorrentEntriesService).toSelf();
        container.bind<ILoggingService>(IocTypes.ILoggingService).toConstantValue(mockLoggingService);
        container.bind<ITorrentFileService>(IocTypes.ITorrentFileService).toConstantValue(mockFileService);
        container.bind<ITorrentSubtitleService>(IocTypes.ITorrentSubtitleService).toConstantValue(mockSubtitleService);
        container.bind<IDatabaseRepository>(IocTypes.IDatabaseRepository).toConstantValue(mockDatabaseRepository);
        container.bind<IMetadataService>(IocTypes.IMetadataService).toConstantValue(mockMetadataService);
        torrentEntriesService = container.get(TorrentEntriesService);
    });

    it('should create a torrent entry', async () => {
        const torrent: IParsedTorrent = {
            title: 'Test title',
            provider: 'Test provider',
            infoHash: 'Test infoHash',
            type: TorrentType.Movie,
        };

        const fileCollection: ITorrentFileCollection = {
            videos: [{
                fileIndex: 0,
                title: 'Test video',
                size: 123456,
                imdbId: 'tt1234567',
            }],
            contents: [],
            subtitles: [],
        };

        const fileCollectionWithSubtitles: ITorrentFileCollection = {
            ...fileCollection,
            subtitles: [{
                fileId: 0,
                title: 'Test subtitle',
                fileIndex: 0,
                path: 'Test path',
                infoHash: 'Test infoHash',
            }],
        };

        (mockMetadataService.getImdbId as jest.Mock).mockResolvedValue('tt1234567');
        (mockFileService.parseTorrentFiles as jest.Mock).mockResolvedValue(fileCollection);
        (mockSubtitleService.assignSubtitles as jest.Mock).mockResolvedValue(fileCollectionWithSubtitles);
        (mockDatabaseRepository.createTorrent as jest.Mock).mockResolvedValue(torrent);

        await torrentEntriesService.createTorrentEntry(torrent);

        expect(mockMetadataService.getImdbId).toHaveBeenCalledWith({
            title: 'Test title',
            year: undefined,
            type: TorrentType.Movie
        });
        expect(mockFileService.parseTorrentFiles).toHaveBeenCalledWith(torrent);
        expect(mockFileService.parseTorrentFiles).toHaveReturnedWith(Promise.resolve(fileCollection));
        expect(mockSubtitleService.assignSubtitles).toHaveBeenCalledWith(fileCollection);
        expect(mockSubtitleService.assignSubtitles).toHaveReturnedWith(Promise.resolve(fileCollectionWithSubtitles));
        expect(mockDatabaseRepository.createTorrent).toHaveBeenCalledWith(expect.objectContaining(torrent));
    });

    it('should assign imdbId correctly', async () => {
        const torrent: IParsedTorrent = {
            title: 'Test title',
            provider: 'Test provider',
            infoHash: 'Test infoHash',
            type: TorrentType.Movie,
        };

        const fileCollection: ITorrentFileCollection = {
            videos: [{
                fileIndex: 0,
                title: 'Test video',
                size: 123456,
                imdbId: 'tt1234567',
            }],
            contents: [],
            subtitles: [],
        };

        const fileCollectionWithSubtitles: ITorrentFileCollection = {
            ...fileCollection,
            subtitles: [{
                fileId: 0,
                title: 'Test subtitle',
                fileIndex: 0,
                path: 'Test path',
                infoHash: 'Test infoHash',
            }],
        };

        (mockMetadataService.getImdbId as jest.Mock).mockResolvedValue('tt1234567');
        (mockFileService.parseTorrentFiles as jest.Mock).mockResolvedValue(fileCollection);
        (mockSubtitleService.assignSubtitles as jest.Mock).mockResolvedValue(fileCollectionWithSubtitles);
        (mockDatabaseRepository.createTorrent as jest.Mock).mockResolvedValue(torrent);

        await torrentEntriesService.createTorrentEntry(torrent);

        expect(torrent.imdbId).toEqual('tt1234567');
        expect(torrent.kitsuId).toEqual(undefined);
    });

    it('should assign kitsuId correctly', async () => {
        const torrent: IParsedTorrent = {
            title: 'Test title',
            provider: 'Test provider',
            infoHash: 'Test infoHash',
            type: TorrentType.Anime,
        };

        const fileCollection: ITorrentFileCollection = {
            videos: [{
                fileIndex: 0,
                title: 'Test video',
                size: 123456,
                kitsuId: 11
            }],
            contents: [],
            subtitles: [],
        };

        const fileCollectionWithSubtitles: ITorrentFileCollection = {
            ...fileCollection,
            subtitles: [{
                fileId: 0,
                title: 'Test subtitle',
                fileIndex: 0,
                path: 'Test path',
                infoHash: 'Test infoHash',
            }],
        };

        (mockMetadataService.getKitsuId as jest.Mock).mockResolvedValue(11);
        (mockFileService.parseTorrentFiles as jest.Mock).mockResolvedValue(fileCollection);
        (mockSubtitleService.assignSubtitles as jest.Mock).mockResolvedValue(fileCollectionWithSubtitles);
        (mockDatabaseRepository.createTorrent as jest.Mock).mockResolvedValue(torrent);

        await torrentEntriesService.createTorrentEntry(torrent);

        expect(torrent.imdbId).toEqual(undefined);
        expect(torrent.kitsuId).toEqual(11);
    });

    it('should create a skip torrent entry', async () => {
        const torrent: ITorrentCreationAttributes = {
            infoHash: 'Test infoHash',
            provider: 'Test provider',
            title: 'Test title',
            type: TorrentType.Movie,
        };

        (mockDatabaseRepository.createSkipTorrent as jest.Mock).mockResolvedValue([torrent, null]);

        const result = await torrentEntriesService.createSkipTorrentEntry(torrent);

        expect(mockDatabaseRepository.createSkipTorrent).toHaveBeenCalledWith(torrent);
        expect(result).toEqual([torrent, null]);
    });

    it('should get stored torrent entry', async () => {
        const torrent = {
            infoHash: 'Test infoHash',
            provider: 'Test provider',
            title: 'Test title',
            type: TorrentType.Movie,
            dataValues: {
                infoHash: 'Test infoHash',
                provider: 'Test provider',
                title: 'Test title',
                type: TorrentType.Movie,
            }
        } as Torrent;

        (mockDatabaseRepository.getSkipTorrent as jest.Mock).mockRejectedValue(undefined);
        (mockDatabaseRepository.getTorrent as jest.Mock).mockResolvedValue(torrent);

        const result = await torrentEntriesService.getStoredTorrentEntry(torrent);

        expect(mockDatabaseRepository.getSkipTorrent).toHaveBeenCalledWith(torrent.infoHash);
        expect(mockDatabaseRepository.getTorrent).toHaveBeenCalledWith(torrent.dataValues);
        expect(result).toEqual(torrent);
    });

    it('should check and update torrent', async () => {
        const torrent: IParsedTorrent = {
            title: 'Test title',
            provider: 'Test provider',
            infoHash: 'Test infoHash',
            type: TorrentType.Movie,
            seeders: 1,
        };

        const files: IFileAttributes[] = [{
            infoHash: 'Test infoHash',
            fileIndex: 0,
            title: 'Test title',
            path: 'Test path',
            size: 123456,
        }, {
            infoHash: 'Test infoHash 2',
            fileIndex: 1,
            title: 'Test title 2',
            path: 'Test path 2',
            size: 234567,
        }];

        const torrentInstance = {
            ...torrent,
            dataValues: {...torrent},
            save: jest.fn().mockResolvedValue(torrent),
        };

        const filesInstance = {
            ...files,
            dataValues: {...files},
            save: jest.fn().mockResolvedValue(files),
        };

        const seedersResponse = [1];

        (mockDatabaseRepository.getTorrent as jest.Mock).mockResolvedValue(torrentInstance);

        (mockDatabaseRepository.setTorrentSeeders as jest.Mock).mockResolvedValue(seedersResponse);
        (mockDatabaseRepository.getFiles as jest.Mock).mockResolvedValue(filesInstance)

        const result = await torrentEntriesService.checkAndUpdateTorrent(torrent);

        expect(mockDatabaseRepository.getTorrent).toHaveBeenCalledWith({
            infoHash: torrent.infoHash,
            provider: torrent.provider
        });

        expect(mockDatabaseRepository.getFiles).toHaveBeenCalledWith(torrent.infoHash);
        expect(mockDatabaseRepository.setTorrentSeeders).toHaveBeenCalledWith(torrentInstance.dataValues, 1);
        expect(result).toEqual(true);
    });

    it('should create torrent contents', async () => {
        const torrent = {
            infoHash: 'Test infoHash',
            provider: 'Test provider',
            title: 'Test title',
            type: TorrentType.Movie,
            dataValues: {
                infoHash: 'Test infoHash',
                provider: 'Test provider',
                title: 'Test title',
                type: TorrentType.Movie,
            }
        } as Torrent;

        const fileCollection: ITorrentFileCollection = {
            videos: [{
                id: 1,
                title: 'Test video',
                size: 123456,
                imdbId: 'tt1234567',
                infoHash: 'Test infoHash',
            }],
            contents: [],
            subtitles: [],
        };

        const fileCollectionWithContents: ITorrentFileCollection = {
            ...fileCollection,
            contents: [{
                size: 123456,
                fileIndex: 0,
                path: 'Test path',
                infoHash: 'Test infoHash',
            }],
        };

        (mockDatabaseRepository.getFiles as jest.Mock).mockResolvedValue(fileCollection.videos);
        (mockFileService.parseTorrentFiles as jest.Mock).mockResolvedValue(fileCollectionWithContents);
        (mockSubtitleService.assignSubtitles as jest.Mock).mockResolvedValue(fileCollectionWithContents);
        (mockDatabaseRepository.createFile as jest.Mock).mockResolvedValue(Promise.resolve());
        (mockDatabaseRepository.createTorrent as jest.Mock).mockResolvedValue(torrent);

        await torrentEntriesService.createTorrentContents(torrent);

        const newTorrentFiles = await (mockDatabaseRepository.createTorrent as jest.Mock).mock.calls[0][0].files;

        newTorrentFiles.forEach(file => {
            expect(mockDatabaseRepository.createFile).toHaveBeenCalledWith(file);
        });
    });
});