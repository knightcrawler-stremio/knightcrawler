import "reflect-metadata"; // required
import {TorrentType} from "@enums/torrent_types";
import {ILoggingService} from "@interfaces/logging_service";
import {IMetadataService} from "@interfaces/metadata_service";
import {IParsedTorrent} from "@interfaces/parsed_torrent";
import {ITorrentFileCollection} from "@interfaces/torrent_file_collection";
import {ITorrentFileService} from "@interfaces/torrent_file_service";
import {ITorrentSubtitleService} from "@interfaces/torrent_subtitle_service";
import {IDatabaseRepository} from "@repository/interfaces/database_repository"; 
import {TorrentEntriesService} from "@services/torrent_entries_service";



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
        parseTorrentFiles: jest.fn(),
    }
})

jest.mock('@services/metadata_service', () => {
    return {
        getImdbId: jest.fn(),
    }
})

jest.mock('@services/torrent_subtitle_service', () => {
    return {
        assignSubtitles: jest.fn(),
    }
})

jest.mock('@repository/database_repository', () => {
    return {
        createTorrent: jest.fn().mockResolvedValue(undefined),
        createFile: jest.fn().mockResolvedValue(undefined),
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
        mockFileService = jest.requireMock<ITorrentFileService>('@services/torrent_file_service');
        mockMetadataService = jest.requireMock<IMetadataService>('@services/metadata_service');
        mockSubtitleService = jest.requireMock<ITorrentSubtitleService>('@services/torrent_subtitle_service');
        mockLoggingService = jest.requireMock<ILoggingService>('@services/logging_service');
        mockDatabaseRepository = jest.requireMock<IDatabaseRepository>('@repository/database_repository');
        torrentEntriesService = new TorrentEntriesService(mockMetadataService, mockLoggingService, mockFileService , mockSubtitleService, mockDatabaseRepository);        
    });

    it('should create a torrent entry', async () => {
        const torrent : IParsedTorrent = {
            title: 'Test title',
            provider: 'Test provider',
            infoHash: 'Test infoHash',
            type: TorrentType.Movie,
        };

        const fileCollection : ITorrentFileCollection = {
            videos: [{
                fileIndex: 0,
                title: 'Test video',
                size: 123456,
                imdbId: 'tt1234567',
            }],
            contents: [],
            subtitles: [],
        };

        const fileCollectionWithSubtitles : ITorrentFileCollection = {
            ...fileCollection,
            subtitles: [ {
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

        expect(mockMetadataService.getImdbId).toHaveBeenCalledWith({ title: 'Test title', year: undefined, type: TorrentType.Movie });
        expect(mockFileService.parseTorrentFiles).toHaveBeenCalledWith(torrent);
        expect(mockFileService.parseTorrentFiles).toHaveReturnedWith(Promise.resolve(fileCollection));
        expect(mockSubtitleService.assignSubtitles).toHaveBeenCalledWith(fileCollection);
        expect(mockSubtitleService.assignSubtitles).toHaveReturnedWith(Promise.resolve(fileCollectionWithSubtitles));
        expect(mockDatabaseRepository.createTorrent).toHaveBeenCalledWith(expect.objectContaining(torrent));
    });
});