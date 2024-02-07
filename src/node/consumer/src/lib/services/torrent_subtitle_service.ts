import {parse} from 'parse-torrent-title';
import {ITorrentFileCollection} from "../interfaces/torrent_file_collection";
import {IFileAttributes} from "../../repository/interfaces/file_attributes";
import {ITorrentSubtitleService} from "../interfaces/torrent_subtitle_service";

class TorrentSubtitleService implements ITorrentSubtitleService {
    public assignSubtitles(fileCollection: ITorrentFileCollection): ITorrentFileCollection {
        if (fileCollection.videos && fileCollection.videos.length && fileCollection.subtitles && fileCollection.subtitles.length) {
            if (fileCollection.videos.length === 1) {
                fileCollection.videos[0].subtitles = fileCollection.subtitles;
                return {...fileCollection, subtitles: []};
            }

            const parsedVideos = fileCollection.videos.map(video => this.parseVideo(video));
            const assignedSubs = fileCollection.subtitles.map(subtitle => ({
                subtitle,
                videos: this.mostProbableSubtitleVideos(subtitle, parsedVideos)
            }));
            const unassignedSubs = assignedSubs.filter(assignedSub => !assignedSub.videos).map(assignedSub => assignedSub.subtitle);

            assignedSubs
                .filter(assignedSub => assignedSub.videos)
                .forEach(assignedSub => assignedSub.videos.forEach(video => video.subtitles = (video.subtitles || []).concat(assignedSub.subtitle)));
            return {...fileCollection, subtitles: unassignedSubs};
        }
        return fileCollection;
    }

    private parseVideo(video: IFileAttributes) {
        const fileName = video.title.split('/').pop().replace(/\.(\w{2,4})$/, '');
        const folderName = video.title.replace(/\/?[^/]+$/, '');
        return {
            videoFile: video,
            fileName: fileName,
            folderName: folderName,
            ...this.parseFilename(video.title)
        };
    }

    private mostProbableSubtitleVideos(subtitle: any, parsedVideos: any[]) {
        const subTitle = (subtitle.title || subtitle.path).split('/').pop().replace(/\.(\w{2,4})$/, '');
        const parsedSub = this.parsePath(subtitle.title || subtitle.path);
        const byFileName = parsedVideos.filter(video => subTitle.includes(video.fileName));
        if (byFileName.length === 1) {
            return byFileName.map(v => v.videoFile);
        }
        const byTitleSeasonEpisode = parsedVideos.filter(video => video.title === parsedSub.title
            && this.arrayEquals(video.seasons, parsedSub.seasons)
            && this.arrayEquals(video.episodes, parsedSub.episodes));
        if (this.singleVideoFile(byTitleSeasonEpisode)) {
            return byTitleSeasonEpisode.map(v => v.videoFile);
        }
        const bySeasonEpisode = parsedVideos.filter(video => this.arrayEquals(video.seasons, parsedSub.seasons)
            && this.arrayEquals(video.episodes, parsedSub.episodes));
        if (this.singleVideoFile(bySeasonEpisode)) {
            return bySeasonEpisode.map(v => v.videoFile);
        }
        const byTitle = parsedVideos.filter(video => video.title && video.title === parsedSub.title);
        if (this.singleVideoFile(byTitle)) {
            return byTitle.map(v => v.videoFile);
        }
        const byEpisode = parsedVideos.filter(video => this.arrayEquals(video.episodes, parsedSub.episodes));
        if (this.singleVideoFile(byEpisode)) {
            return byEpisode.map(v => v.videoFile);
        }
        return undefined;
    }

    private singleVideoFile(videos: any[]) {
        return new Set(videos.map(v => v.videoFile.fileIndex)).size === 1;
    }

    private parsePath(path: string) {
        const pathParts = path.split('/').map(part => this.parseFilename(part));
        const parsedWithEpisode = pathParts.find(parsed => parsed.season && parsed.episodes);
        return parsedWithEpisode || pathParts[pathParts.length - 1];
    }

    private parseFilename(filename: string) {
        const parsedInfo = parse(filename)
        const titleEpisode = parsedInfo.title.match(/(\d+)$/);
        if (!parsedInfo.episodes && titleEpisode) {
            parsedInfo.episodes = [parseInt(titleEpisode[1], 10)];
        }
        return parsedInfo;
    }

    private arrayEquals(array1: any[], array2: any[]) {
        if (!array1 || !array2) return array1 === array2;
        return array1.length === array2.length && array1.every((value, index) => value === array2[index])
    }
}

export const torrentSubtitleService = new TorrentSubtitleService();
