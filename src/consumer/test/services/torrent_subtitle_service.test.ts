import "reflect-metadata"; // required
import {ITorrentFileCollection} from "@interfaces/torrent_file_collection";
import {TorrentSubtitleService} from "@services/torrent_subtitle_service";

describe('TorrentSubtitleService tests', () => {
    let torrentSubtitleService: TorrentSubtitleService;

    beforeEach(() => {
        jest.clearAllMocks();
        torrentSubtitleService = new TorrentSubtitleService();
    });

    it('should assign subtitles to a single video', () => {
        const fileCollection: ITorrentFileCollection = {
            videos: [{title: 'Test video', size: 123456, imdbId: 'tt1234567', infoHash: 'Test infoHash'}],
            contents: [],
            subtitles: [{title: 'Test subtitle', fileIndex: 0, path: 'Test path', infoHash: 'Test infoHash'}],
        };

        const result = torrentSubtitleService.assignSubtitles(fileCollection);

        expect(result.videos[0].subtitles).toEqual(fileCollection.subtitles);
        expect(result.subtitles).toEqual([]);
    });

    it('should not assign subtitles if there are no videos', () => {
        const fileCollection: ITorrentFileCollection = {
            videos: [],
            contents: [],
            subtitles: [{title: 'Test subtitle', fileIndex: 0, path: 'Test path', infoHash: 'Test infoHash'}],
        };

        const result = torrentSubtitleService.assignSubtitles(fileCollection);

        expect(result).toEqual(fileCollection);
    });

    it('should not assign subtitles if there are no subtitles', () => {
        const fileCollection: ITorrentFileCollection = {
            videos: [{title: 'Test video', size: 123456, imdbId: 'tt1234567', infoHash: 'Test infoHash'}],
            contents: [],
            subtitles: [],
        };

        const result = torrentSubtitleService.assignSubtitles(fileCollection);

        expect(result).toEqual(fileCollection);
    });

    it('should assign subtitles to multiple videos', () => {
        const fileCollection: ITorrentFileCollection = {
            videos: [
                {title: 'Test video S01E01', size: 123456, imdbId: 'tt1234567', infoHash: 'Test infoHash'},
                {title: 'Test video S01E02', size: 123456, imdbId: 'tt1234567', infoHash: 'Test infoHash'}
            ],
            contents: [],
            subtitles: [
                {title: 'Test subtitle S01E01', fileIndex: 0, path: 'Test path', infoHash: 'Test infoHash'},
                {title: 'Test subtitle S01E02', fileIndex: 1, path: 'Test path', infoHash: 'Test infoHash'}
            ],
        };

        const result = torrentSubtitleService.assignSubtitles(fileCollection);

        expect(result.videos[0].subtitles).toEqual([fileCollection.subtitles[0]]);
        expect(result.videos[1].subtitles).toEqual([fileCollection.subtitles[1]]);
        expect(result.subtitles).toEqual([]);
    });

    it('should not assign subtitles if there are no matching videos', () => {
        const fileCollection: ITorrentFileCollection = {
            videos: [{title: 'Test video', size: 123456, imdbId: 'tt1234567', infoHash: 'Test infoHash'}],
            contents: [],
            subtitles: [{title: 'Non-matching subtitle', fileIndex: 0, path: 'Test path', infoHash: 'Non-matching infoHash'}],
        };

        const result = torrentSubtitleService.assignSubtitles(fileCollection);

        expect(result.videos[0].subtitles).toEqual([]);
        expect(result.subtitles).toEqual([fileCollection.subtitles[0]]);
    });

    it('should assign subtitles to the most probable videos based on filename, title, season, and episode', () => {
        const fileCollection: ITorrentFileCollection = {
            videos: [
                {title: 'Test video S01E01', size: 123456, imdbId: 'tt1234567', infoHash: 'Test infoHash'},
                {title: 'Test video S01E02', size: 123456, imdbId: 'tt1234567', infoHash: 'Test infoHash'}
            ],
            contents: [],
            subtitles: [
                {title: 'Test subtitle S01E01', fileIndex: 0, path: 'Test path', infoHash: 'Test infoHash'},
                {title: 'Test subtitle S01E02', fileIndex: 1, path: 'Test path', infoHash: 'Test infoHash'}
            ],
        };

        const result = torrentSubtitleService.assignSubtitles(fileCollection);

        expect(result.videos[0].subtitles).toEqual([fileCollection.subtitles[0]]);
        expect(result.videos[1].subtitles).toEqual([fileCollection.subtitles[1]]);
        expect(result.subtitles).toEqual([]);
    });
});
