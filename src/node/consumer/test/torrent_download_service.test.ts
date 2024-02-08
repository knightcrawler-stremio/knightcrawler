import "reflect-metadata"; // required
import { ILoggingService } from '@interfaces/logging_service';
import {IParsedTorrent} from "@interfaces/parsed_torrent";
import { TorrentDownloadService } from '@services/torrent_download_service';
import torrentStream from 'torrent-stream';

jest.mock('@services/logging_service', () => {
    return {
        error: jest.fn(),
        info: jest.fn(),
        debug: jest.fn(),
    }
})

jest.mock('torrent-stream', () => {
    return jest.fn().mockImplementation(() => ({
        on: jest.fn(),
        files: [],
        destroy: jest.fn(),
    }));
});

describe('TorrentDownloadService', () => {
    let torrentDownloadService: TorrentDownloadService,
     mockLoggingService: ILoggingService;

    beforeEach(() => {
        mockLoggingService = jest.requireMock<ILoggingService>('@services/logging_service');
        torrentDownloadService = new TorrentDownloadService(mockLoggingService);
    });

    it('should get torrent files', async () => {
        const mockTorrent: IParsedTorrent = {
            size: 123456789,
            isPack: false,
            imdbId: 'tt1234567',
            kitsuId: 1234,
            trackers: 'http://tracker1.com,http://tracker2.com',
            provider: 'provider1',
            infoHash: '1234567890abcdef',
            type: 'movie',
            uploadDate: new Date(),
            seeders: 100,
            torrentId: 'torrent1',
            fileCollection: { },
            title: 'Test Movie',
            year: 2020,
            season: 1,
            episode: 1,
            resolution: '1080p',
            codec: 'H.264',
            audio: 'AAC',
            group: 'GRP',
            extended: false,
            hardcoded: false,
            proper: false,
            repack: false,
            container: 'mp4',
            unrated: false,
        };
        
        const mockFiles = [
            {
                name: 'file1.mp4',
                path: '/path/to/file1.mp4',
                length: 123456789,
                fileIndex: 0,
                select: jest.fn(),
                deselect: jest.fn(),
                createReadStream: jest.fn(),
            },
            {
                name: 'file2.srt',
                path: '/path/to/file2.srt',
                length: 987654321,
                fileIndex: 1,
                select: jest.fn(),
                deselect: jest.fn(),
                createReadStream: jest.fn(),
            },
        ];


        const mockEngine = {
            on: jest.fn((event, callback) => {
                if (event === 'ready') {
                    callback();
                }
            }),
            files: mockFiles,
            destroy: jest.fn(),
            connect: jest.fn(),
            disconnect: jest.fn(),
            block: jest.fn(),
            remove: jest.fn(),
            listen: jest.fn(),
            swarm: {
                downloaded: 2,
            },
            infoHash: 'mockInfoHash',
        };

        (torrentStream as jest.MockedFunction<typeof torrentStream>).mockReturnValue(mockEngine);


        const result = await torrentDownloadService.getTorrentFiles(mockTorrent);

        expect(result).toEqual({
            contents: mockFiles.map(file => ({
                path: file.path,
                fileIndex: file.fileIndex,
                infoHash: mockTorrent.infoHash,
                size: file.length
            })),
            videos: mockFiles.filter(file => file.name.endsWith('.mp4')).map(file => ({
                fileIndex: file.fileIndex,
                infoHash: mockTorrent.infoHash,
                path: file.path,
                size: file.length,
                title: file.name.split('.')[0],
                extension: file.name.split('.')[1],
                container: file.name.split('.')[1],
                imdbId: mockTorrent.imdbId,
                imdbSeason: mockTorrent.season,
                imdbEpisode: mockTorrent.episode,
                kitsuId: mockTorrent.kitsuId,
                kitsuEpisode: mockTorrent.episode,
            })),
            subtitles: mockFiles.filter(file => file.name.endsWith('.srt')).map(file => ({
                fileIndex: file.fileIndex,
                infoHash: mockTorrent.infoHash,
                path: file.path,
                title: file.name,
                fileId: file.fileIndex,
            })),
        });
        
        expect(torrentStream).toHaveBeenCalledWith(expect.any(String), expect.any(Object));
        expect(mockLoggingService.debug).toHaveBeenCalledWith(`Adding torrent with infoHash ${mockTorrent.infoHash} to torrent engine...`);
    });
});