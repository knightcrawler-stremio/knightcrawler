import "reflect-metadata"; // required
import {TorrentType} from "@enums/torrent_types";
import {ILoggingService} from "@interfaces/logging_service";
import {IMetadataService} from "@interfaces/metadata_service";
import {IParsedTorrent} from "@interfaces/parsed_torrent";
import {ITorrentDownloadService} from "@interfaces/torrent_download_service";
import {TorrentFileService} from "@services/torrent_file_service";

jest.mock('@services/logging_service', () => {
    return {
        error: jest.fn(),
        info: jest.fn(),
        debug: jest.fn(),
    }
})

jest.mock('@services/torrent_download_service', () => {
    return {
        getTorrentFiles: jest.fn().mockImplementation(() => Promise.resolve({
            contents: [],
            videos: [],
            subtitles: [],
        })),
    };
});

jest.mock('@services/metadata_service', () => {
    return {
        getMetadata: jest.fn().mockImplementation(() => Promise.resolve(undefined)),
    }
});

describe('TorrentFileService tests', () => {
    let torrentFileService: TorrentFileService,
        mockLoggingService: ILoggingService,
        mockDownloadService: ITorrentDownloadService,
        mockMetadataService: IMetadataService;

    beforeEach(() => {
        jest.clearAllMocks();
        mockLoggingService = jest.requireMock<ILoggingService>('@services/logging_service');
        mockDownloadService = jest.requireMock<ITorrentDownloadService>('@services/torrent_download_service');
        mockMetadataService = jest.requireMock<IMetadataService>('@services/metadata_service');
        torrentFileService = new TorrentFileService(mockMetadataService, mockDownloadService, mockLoggingService);
    });


    it('should parse torrent files correctly', () => {
        const mockTorrent: IParsedTorrent = {
            title: 'test',
            kitsuId: 123,
            type: TorrentType.Movie,
            infoHash: '1234567890abcdef',
        };

        const result = torrentFileService.parseTorrentFiles(mockTorrent);

        expect(result).toBeInstanceOf(Promise);
        
        result.then(res => {
            expect(res).toHaveProperty('videos');
            expect(res).toHaveProperty('subtitles');
            expect(res).toHaveProperty('contents');
        });
    });

    it('should reject when torrent has no title', async () => {
        const mockTorrent: IParsedTorrent = {
            kitsuId: 123,
            type: TorrentType.Movie,
            infoHash: '1234567890abcdef',
        };

        await expect(torrentFileService.parseTorrentFiles(mockTorrent)).rejects.toThrow('Torrent title is missing');
    });

    it('should handle torrent with no kitsuId', async () => {
        const mockTorrent: IParsedTorrent = {
            title: 'test',
            type: TorrentType.Movie,
            infoHash: '1234567890abcdef',
        };

        const result = await torrentFileService.parseTorrentFiles(mockTorrent);

        expect(result).toHaveProperty('videos');
        expect(result).toHaveProperty('subtitles');
        expect(result).toHaveProperty('contents');
    });

    it('should handle torrent of type Series', async () => {
        const mockTorrent: IParsedTorrent = {
            title: 'test',
            kitsuId: 123,
            type: TorrentType.Series,
            infoHash: '1234567890abcdef',
        };

        const result = await torrentFileService.parseTorrentFiles(mockTorrent);

        expect(result).toHaveProperty('videos');
        expect(result).toHaveProperty('subtitles');
        expect(result).toHaveProperty('contents');
    });

    it('should reject when torrent has no infoHash', async () => {
        const mockTorrent = {
            title: 'test',
            kitsuId: 123,
            type: TorrentType.Movie,
        } as IParsedTorrent;

        await expect(torrentFileService.parseTorrentFiles(mockTorrent)).rejects.toThrow('Torrent infoHash is missing');
    });

    it('should handle torrent with no type', async () => {
        const mockTorrent = {
            title: 'test',
            kitsuId: 123,
            infoHash: '1234567890abcdef',
        } as IParsedTorrent;

        const result = await torrentFileService.parseTorrentFiles(mockTorrent);

        expect(result).toHaveProperty('videos');
        expect(result).toHaveProperty('subtitles');
        expect(result).toHaveProperty('contents');
    });

    it('should handle torrent of type Anime', async () => {
        const mockTorrent: IParsedTorrent = {
            title: 'test',
            kitsuId: 123,
            type: TorrentType.Anime,
            infoHash: '1234567890abcdef',
        };

        const result = await torrentFileService.parseTorrentFiles(mockTorrent);

        expect(result).toHaveProperty('videos');
        expect(result).toHaveProperty('subtitles');
        expect(result).toHaveProperty('contents');
    });

    it('should handle torrent with a single video', async () => {
        const mockTorrent: IParsedTorrent = {
            title: 'test',
            kitsuId: 123,
            type: TorrentType.Movie,
            infoHash: '1234567890abcdef',
        };

        (mockDownloadService.getTorrentFiles as jest.Mock).mockImplementation(() => Promise.resolve({
            contents: [],
            videos: [{
                title: 'video1',
                path: 'path/to/video1',
                size: 123456789,
                fileIndex: 0,
            }],
            subtitles: [],
        }));

        const result = await torrentFileService.parseTorrentFiles(mockTorrent);

        expect(result).toHaveProperty('videos');
        expect(result.videos).toHaveLength(1);
        expect(result.videos[0]).toHaveProperty('title', 'video1');
        expect(result).toHaveProperty('subtitles');
        expect(result).toHaveProperty('contents');
    });

    it('should handle torrent with multiple videos', async () => {
        const mockTorrent: IParsedTorrent = {
            title: 'test',
            kitsuId: 123,
            type: TorrentType.Movie,
            infoHash: '1234567890abcdef',
        };

        (mockDownloadService.getTorrentFiles as jest.Mock).mockImplementation(() => Promise.resolve({
            contents: [],
            videos: [
                {
                    title: 'video1',
                    path: 'path/to/video1',
                    size: 123456789,
                    fileIndex: 0,
                },
                {
                    title: 'video2',
                    path: 'path/to/video2',
                    size: 123456789,
                    fileIndex: 1,
                },
                {
                    title: 'video3',
                    path: 'path/to/video3',
                    size: 123456789,
                    fileIndex: 2,
                }
            ],
            subtitles: [],
        }));

        const result = await torrentFileService.parseTorrentFiles(mockTorrent);

        expect(result).toHaveProperty('videos');
        expect(result.videos).toHaveLength(3);
        expect(result.videos[0]).toHaveProperty('title', 'video1');
        expect(result.videos[1]).toHaveProperty('title', 'video2');
        expect(result.videos[2]).toHaveProperty('title', 'video3');
        expect(result).toHaveProperty('subtitles');
        expect(result).toHaveProperty('contents');
    });
});