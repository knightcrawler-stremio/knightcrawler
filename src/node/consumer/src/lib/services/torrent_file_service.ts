import {TorrentType} from '@enums/torrent_types';
import {ExtensionHelpers} from '@helpers/extension_helpers';
import {ILoggingService} from "@interfaces/logging_service";
import {IMetadataService} from "@interfaces/metadata_service";
import {IParsedTorrent} from "@interfaces/parsed_torrent";
import {ITorrentDownloadService} from "@interfaces/torrent_download_service";
import {ITorrentFileCollection} from "@interfaces/torrent_file_collection";
import {ITorrentFileService} from "@interfaces/torrent_file_service";
import {IContentAttributes} from "@repository/interfaces/content_attributes";
import {IFileAttributes} from "@repository/interfaces/file_attributes";
import {configurationService} from '@services/configuration_service';
import {IocTypes} from "@setup/ioc_types";
import {inject, injectable} from "inversify";
import {parse} from 'parse-torrent-title';
import { filenameParse } from '@ctrl/video-filename-parser';
import {ParsedShow} from "@ctrl/video-filename-parser/dist/src/filenameParse";

const MIN_SIZE: number = 5 * 1024 * 1024; // 5 MB
const MULTIPLE_FILES_SIZE = 4 * 1024 * 1024 * 1024; // 4 GB

@injectable()
export class TorrentFileService implements ITorrentFileService {
    @inject(IocTypes.IMetadataService) metadataService: IMetadataService;
    @inject(IocTypes.ITorrentDownloadService) torrentDownloadService: ITorrentDownloadService;
    @inject(IocTypes.ILoggingService) logger: ILoggingService;

    async parseTorrentFiles(torrent: IParsedTorrent): Promise<ITorrentFileCollection> {
        if (!torrent.title) {
            return Promise.reject(new Error('Torrent title is missing'));
        }

        if (!torrent.infoHash) {
            return Promise.reject(new Error('Torrent infoHash is missing'));
        }

        let fileCollection: ITorrentFileCollection;

        const isSeries = parse(torrent.title).seasons || this.isSeries(torrent.title);

        if (!isSeries){
            fileCollection = await this.parseMovieFiles(torrent);
        } else {
            fileCollection = await this.parseSeriesFiles(torrent);
        }

        return fileCollection;
    }

    isPackTorrent(torrent: IParsedTorrent): boolean {
        if (torrent.isPack) {
            return true;
        }
        if (!torrent.title) {
            return false;
        }
        const parsedInfo = parse(torrent.title);
        if (torrent.type === TorrentType.Movie) {
            return parsedInfo.complete || typeof parsedInfo.year === 'string' || /movies/i.test(torrent.title);
        }

        const hasMultipleEpisodes = Boolean(parsedInfo.complete || torrent.size || 0 > MULTIPLE_FILES_SIZE ||
            (parsedInfo.seasons && parsedInfo.seasons.length > 1) ||
            (parsedInfo.episodes && parsedInfo.episodes.length > 1) ||
            (parsedInfo.seasons && !parsedInfo.episodes));

        const hasSingleEpisode: boolean = Boolean(Number.isInteger(parsedInfo.episode) || (!parsedInfo.episodes && parsedInfo.date));

        return hasMultipleEpisodes && !hasSingleEpisode;
    }

    private parseSeriesVideos = (torrent: IParsedTorrent, videos: IFileAttributes[]): IFileAttributes[] => {
        const parsedTorrentName = parse(torrent.title!);
        const hasMovies = parsedTorrentName.complete || !!torrent.title!.match(/movies?(?:\W|$)/i);
        const parsedVideos = videos.map(video => this.parseSeriesVideo(video));

        return parsedVideos.map(video => ({...video, isMovie: this.isMovieVideo(torrent, video, parsedVideos, hasMovies)}));
    };

    private parseMovieFiles = async (torrent: IParsedTorrent): Promise<ITorrentFileCollection> => {
        const fileCollection: ITorrentFileCollection = await this.getMoviesTorrentContent(torrent);
        if (fileCollection.videos === undefined || fileCollection.videos.length === 0) {
            return {...fileCollection, videos: this.getDefaultFileEntries(torrent)};
        }

        const filteredVideos = fileCollection.videos
            .filter(video => video.size! > MIN_SIZE)
            .filter(video => !this.isFeaturette(video));
        if (this.isSingleMovie(filteredVideos)) {
            const parsedVideos = filteredVideos.map(video => ({
                infoHash: torrent.infoHash,
                fileIndex: video.fileIndex,
                title: video.title || video.path || video.fileName || '',
                size: video.size || torrent.size,
                imdbId: torrent.imdbId?.toString(),
                kitsuId: parseInt(torrent.kitsuId?.toString() || '0')
            }));
            return {...fileCollection, videos: parsedVideos};
        }

        const parsedVideos = filteredVideos.map(video => ({
            infoHash: torrent.infoHash,
            fileIndex: video.fileIndex,
            title: video.title || video.path,
            size: video.size,
            imdbId: torrent.imdbId.toString() || ''
        }));

        return {...fileCollection, videos: parsedVideos};
    };

    private parseSeriesFiles = async (torrent: IParsedTorrent): Promise<ITorrentFileCollection> => {
        const fileCollection: ITorrentFileCollection = await this.getSeriesTorrentContent(torrent);
        if (fileCollection.videos === undefined || fileCollection.videos.length === 0) {
            return {...fileCollection, videos: this.getDefaultFileEntries(torrent)};
        }

        const parsedVideos: IFileAttributes[] = await Promise.resolve(fileCollection.videos)
            .then(videos => videos.filter(video => videos?.length === 1 || video.size! > MIN_SIZE))
            .then(videos => this.parseSeriesVideos(torrent, videos))
            .then(videos => videos
                .reduce((a, b) => a.concat(b), [])
                .map(video => this.isFeaturette(video) ? this.clearInfoFields(video) : video))
                .then(videos => Promise.all(videos.flatMap(video => this.mapSeriesEpisode(torrent, video, videos))))
                .then(videos => videos.flat());


        return {...torrent.fileCollection, videos: parsedVideos};
    };

    private getMoviesTorrentContent = async (torrent: IParsedTorrent): Promise<ITorrentFileCollection> => {
        const files = await this.torrentDownloadService.getTorrentFiles(torrent, configurationService.torrentConfig.TIMEOUT)
            .catch(error => {
                if (!this.isPackTorrent(torrent)) {
                    const entries = this.getDefaultFileEntries(torrent);
                    return {videos: entries, contents: [], subtitles: [], files: entries}
                }
                return Promise.reject(error);
            });

        if (files.contents && files.contents.length && !files.videos?.length && this.isDiskTorrent(files.contents)) {
            files.videos = this.getDefaultFileEntries(torrent);
        }

        return files;
    };

    private getDefaultFileEntries = (torrent: IParsedTorrent): IFileAttributes[] => [{
        title: torrent.title!,
        path: torrent.title,
        size: torrent.size,
        fileIndex: 0,
    }];

    private getSeriesTorrentContent = async (torrent: IParsedTorrent): Promise<ITorrentFileCollection> => this.torrentDownloadService.getTorrentFiles(torrent, configurationService.torrentConfig.TIMEOUT)
        .catch(error => {
            if (!this.isPackTorrent(torrent)) {
                return {videos: this.getDefaultFileEntries(torrent), subtitles: [], contents: []}
            }
            return Promise.reject(error);
        });

    private mapSeriesEpisode = async (torrent: IParsedTorrent, file: IFileAttributes, files: IFileAttributes[]): Promise<IFileAttributes[]> => {
        if (!file.episodes && !file.episodes) {
            if (files.length === 1 || files.some(f => f.episodes || f.episodes) || parse(torrent.title!).seasons) {
                return Promise.resolve([{
                    infoHash: torrent.infoHash,
                    fileIndex: file.fileIndex,
                    title: file.path || file.title,
                    size: file.size,
                    imdbId: torrent?.imdbId?.toString() || file?.imdbId?.toString() || '',
                    kitsuId: torrent?.kitsuId || file.kitsuId || 0,
                    imdbSeason: file.imdbSeason,
                    imdbEpisode: file.imdbEpisode,
                    kitsuEpisode: file.kitsuEpisode,
                }]);
            }
            return Promise.resolve([]);
        }
        const episodeIndexes = [...(file.episodes || file.episodes).keys()];
        return Promise.resolve(episodeIndexes.map((index) => ({
            infoHash: torrent.infoHash,
            fileIndex: file.fileIndex,
            title: file.path || file.title,
            size: file.size,
            imdbId: file?.imdbId?.toString() || torrent?.imdbId?.toString() || '',
            imdbSeason: file.imdbSeason,
            season: file.season,
            imdbEpisode: file.episodes && file.episodes[index],
            episode: file.episodes && file.episodes[index],
            kitsuEpisode: file.kitsuId && file.kitsuId !== 0 && file.episodes && file.episodes[index],
            episodes: file.episodes,
            kitsuId: parseInt(file.kitsuId?.toString() || torrent.kitsuId?.toString() || '0') || 0,
        })))
    };

    private isDiskTorrent = (contents: IContentAttributes[]): boolean => contents.some(content => ExtensionHelpers.isDisk(content.path));

    private isSingleMovie = (videos: IFileAttributes[]): boolean => videos.length === 1 ||
        (videos.length === 2 &&
            videos.find(v => /\b(?:part|disc|cd)[ ._-]?0?1\b|^0?1\.\w{2,4}$/i.test(v.path!)) &&
            videos.find(v => /\b(?:part|disc|cd)[ ._-]?0?2\b|^0?2\.\w{2,4}$/i.test(v.path!))) !== undefined;

    private isFeaturette = (video: IFileAttributes): boolean => /featurettes?\/|extras-grym/i.test(video.path!);

    private parseSeriesVideo = (video: IFileAttributes): IFileAttributes => {
        let pathParts = video.path?.split('/');
        let fileName = pathParts[pathParts.length - 1];

        let regexList = [
            { regex: /(saison|season|se|s)\s?(\d{2})/gi, format: (match: RegExpMatchArray) => `S${match[2]}` },
            { regex: /(episode|ep|e)\s?(\d{2})/gi, format: (match: RegExpMatchArray) => `E${match[2]}` },
        ];

        let formattedValues: string[] = [];

        for (let i = 0; i < regexList.length; i++) {
            let regexEntry = regexList[i];
            let match = regexEntry.regex.exec(fileName);
            if (match) {
                let formattedValue = regexEntry.format(match);
                fileName = fileName.replace(match[0], '');
                formattedValues.push(formattedValue);
            }
        }

        fileName = fileName.trim();

        let splitFilename = fileName.split(/\.(?=[^.]*$)/);

        fileName = `${splitFilename[0]} ${formattedValues.join('')}.${splitFilename[1]}`;

        const parsedInfo = filenameParse(fileName, true);

        if ('isTv' in parsedInfo) {
            const parsedShow = parsedInfo as ParsedShow;
            return this.mapParsedShowToVideo(video, parsedShow);
        } else {
            return {
                title: video.title,
                path: video.path,
                size: video.size,
                fileIndex: video.fileIndex,
                imdbId: video.imdbId ? video.imdbId : undefined,
                kitsuId: video.kitsuId && video.kitsuId !== 0 ? video.kitsuId : 0,
            };
        }
    };

    private isSeries = (title: string): boolean => {
        const regexList = [
            /(saison|season|se|s)\s?(\d{2})/gi,
            /(episode|ep|e)\s?(\d{2})/gi,
        ];

        return regexList.some(regex => regex.test(title));
    };

    private mapParsedShowToVideo(video: IFileAttributes, parsedShow: ParsedShow & { isTv: true }) : IFileAttributes {
        let response : IFileAttributes = {
            title: video.title,
            season: parsedShow.seasons[0],
            episode: parsedShow.episodeNumbers[0],
            path: video.path,
            size: video.size,
            fileIndex: video.fileIndex,
            imdbId: video.imdbId ? video.imdbId : undefined,
            kitsuId: video.kitsuId && video.kitsuId !== 0 ? video.kitsuId : 0,
            imdbSeason: video.season,
            imdbEpisode: video.episode,
            kitsuEpisode: video.episode,
        };

        if (!response.imdbSeason && response.imdbEpisode) {
            response.imdbSeason = 0;
        }

        return response;
    }

    private isMovieVideo = (torrent: IParsedTorrent, video: IFileAttributes, otherVideos: IFileAttributes[], hasMovies: boolean): boolean => {
        if (Number.isInteger(torrent.season) && Array.isArray(torrent.episodes)) {
            // not movie if video has season
            return false;
        }
        if (torrent.title?.match(/\b(?:\d+[ .]movie|movie[ .]\d+)\b/i)) {
            // movie if video explicitly has numbered movie keyword in the name, ie. 1 Movie or Movie 1
            return true;
        }
        if (!hasMovies && torrent.type !== TorrentType.Anime) {
            // not movie if torrent name does not contain movies keyword or is not a pack torrent and is not anime
            return false;
        }
        if (!torrent.episodes) {
            // movie if there's no episode info it could be a movie
            return true;
        }
        // movie if contains year info and there aren't more than 3 video with same title and year
        // as some series titles might contain year in it.
        return !!torrent.year
            && otherVideos.length > 3
            && otherVideos.filter(other => other.title === video.title && other.year === video.year).length < 3;
    };

    private clearInfoFields = (video: IFileAttributes): IFileAttributes => {
        video.imdbId = undefined;
        video.imdbSeason = undefined;
        video.imdbEpisode = undefined;
        video.kitsuId = undefined;
        video.kitsuEpisode = undefined;
        return video;
    };
}
