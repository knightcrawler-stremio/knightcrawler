import { parse } from 'parse-torrent-title';
import { TorrentType } from '../enums/torrent_types';
import {ParseTorrentTitleResult} from "../interfaces/parse_torrent_title_result";
import {ParsableTorrentFile} from "../interfaces/parsable_torrent_file";
import {TorrentInfo} from "../interfaces/torrent_info";

class ParsingService {
    private readonly MULTIPLE_FILES_SIZE = 4 * 1024 * 1024 * 1024; // 4 GB

    public parseSeriesVideos(torrent: TorrentInfo, videos: ParsableTorrentFile[]): ParsableTorrentFile[] {
        const parsedTorrentName = parse(torrent.title);
        const hasMovies = parsedTorrentName.complete || !!torrent.title.match(/movies?(?:\W|$)/i);
        const parsedVideos = videos.map(video => this.parseSeriesVideo(video, parsedTorrentName));
        return parsedVideos.map(video => ({ ...video, isMovie: this.isMovieVideo(video, parsedVideos, torrent.type, hasMovies) }));
    }

    public isPackTorrent(torrent: TorrentInfo): boolean {
        if (torrent.pack) {
            return true;
        }
        const parsedInfo = parse(torrent.title);
        if (torrent.type === TorrentType.MOVIE) {
            return parsedInfo.complete || typeof parsedInfo.year === 'string' || /movies/i.test(torrent.title);
        }
        const hasMultipleEpisodes = parsedInfo.complete ||
            torrent.size > this.MULTIPLE_FILES_SIZE ||
            (parsedInfo.seasons && parsedInfo.seasons.length > 1) ||
            (parsedInfo.episodes && parsedInfo.episodes.length > 1) ||
            (parsedInfo.seasons && !parsedInfo.episodes);
        const hasSingleEpisode = Number.isInteger(parsedInfo.episode) || (!parsedInfo.episodes && parsedInfo.date);
        return hasMultipleEpisodes && !hasSingleEpisode;
    }

    private parseSeriesVideo(video: ParsableTorrentFile, parsedTorrentName: ParseTorrentTitleResult): ParseTorrentTitleResult {
        const videoInfo = parse(video.name);
        // the episode may be in a folder containing season number
        if (!Number.isInteger(videoInfo.season) && video.path.includes('/')) {
            const folders = video.path.split('/');
            const pathInfo = parse(folders[folders.length - 2]);
            videoInfo.season = pathInfo.season;
        }
        if (!Number.isInteger(videoInfo.season) && parsedTorrentName.season) {
            videoInfo.season = parsedTorrentName.season;
        }
        if (!Number.isInteger(videoInfo.season) && videoInfo.seasons && videoInfo.seasons.length > 1) {
            // in case single file was interpreted as having multiple seasons
            videoInfo.season = videoInfo.seasons[0];
        }
        if (!Number.isInteger(videoInfo.season) && video.path.includes('/') && parsedTorrentName.seasons
            && parsedTorrentName.seasons.length > 1) {
            // russian season are usually named with 'series name-2` i.e. Улицы разбитых фонарей-6/22. Одиночный выстрел.mkv
            const folderPathSeasonMatch = video.path.match(/[\u0400-\u04ff]-(\d{1,2})(?=.*\/)/);
            videoInfo.season = folderPathSeasonMatch && parseInt(folderPathSeasonMatch[1], 10) || undefined;
        }
        // sometimes video file does not have correct date format as in torrent title
        if (!videoInfo.episodes && !videoInfo.date && parsedTorrentName.date) {
            videoInfo.date = parsedTorrentName.date;
        }
        // limit number of episodes in case of incorrect parsing
        if (videoInfo.episodes && videoInfo.episodes.length > 20) {
            videoInfo.episodes = [videoInfo.episodes[0]];
            videoInfo.episode = videoInfo.episodes[0];
        }
        // force episode to any found number if it was not parsed
        if (!videoInfo.episodes && !videoInfo.date) {
            const epMatcher = videoInfo.title.match(
                /(?<!season\W*|disk\W*|movie\W*|film\W*)(?:^|\W|_)(\d{1,4})(?:a|b|c|v\d)?(?:_|\W|$)(?!disk|movie|film)/i);
            videoInfo.episodes = epMatcher && [parseInt(epMatcher[1], 10)];
            videoInfo.episode = videoInfo.episodes && videoInfo.episodes[0];
        }
        if (!videoInfo.episodes && !videoInfo.date) {
            const epMatcher = video.name.match(new RegExp(`(?:\\(${videoInfo.year}\\)|part)[._ ]?(\\d{1,3})(?:\\b|_)`, "i"));
            videoInfo.episodes = epMatcher && [parseInt(epMatcher[1], 10)];
            videoInfo.episode = videoInfo.episodes && videoInfo.episodes[0];
        }

        return { ...video, ...videoInfo };
    }

    private isMovieVideo(video: ParseTorrentTitleResult, otherVideos: ParseTorrentTitleResult[], type: TorrentType, hasMovies: boolean): boolean {
        if (Number.isInteger(video.season) && Array.isArray(video.episodes)) {
            // not movie if video has season
            return false;
        }
        if (video.title.match(/\b(?:\d+[ .]movie|movie[ .]\d+)\b/i)) {
            // movie if video explicitly has numbered movie keyword in the name, ie. 1 Movie or Movie 1
            return true;
        }
        if (!hasMovies && type !== TorrentType.ANIME) {
            // not movie if torrent name does not contain movies keyword or is not a pack torrent and is not anime
            return false;
        }
        if (!video.episodes) {
            // movie if there's no episode info it could be a movie
            return true;
        }
        // movie if contains year info and there aren't more than 3 video with same title and year
        // as some series titles might contain year in it.
        return !!video.year
            && otherVideos.length > 3
            && otherVideos.filter(other => other.title === video.title && other.year === video.year).length < 3;
    }
}

export const parsingService = new ParsingService();

