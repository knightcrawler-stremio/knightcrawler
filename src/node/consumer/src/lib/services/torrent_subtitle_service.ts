import {ITorrentFileCollection} from "@interfaces/torrent_file_collection";
import {ITorrentSubtitleService} from "@interfaces/torrent_subtitle_service";
import {IFileAttributes} from "@repository/interfaces/file_attributes";
import {ISubtitleAttributes} from "@repository/interfaces/subtitle_attributes";
import {injectable} from "inversify";
import {parse} from 'parse-torrent-title';

@injectable()
export class TorrentSubtitleService implements ITorrentSubtitleService {
    assignSubtitles(fileCollection: ITorrentFileCollection): ITorrentFileCollection {
        if (fileCollection.videos && fileCollection.videos.length && fileCollection.subtitles && fileCollection.subtitles.length) {
            if (fileCollection.videos.length === 1) {
                const matchingSubtitles = fileCollection.subtitles.filter(subtitle =>
                    this.mostProbableSubtitleVideos(subtitle, [fileCollection.videos[0]]).length > 0
                );
                fileCollection.videos[0].subtitles = matchingSubtitles;
                const nonMatchingSubtitles = fileCollection.subtitles.filter(subtitle =>
                    !matchingSubtitles.includes(subtitle)
                );
                return {...fileCollection, subtitles: nonMatchingSubtitles};
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

    private parseVideo = (video: IFileAttributes): IFileAttributes => {
        const fileName = video.title?.split('/')?.pop()?.replace(/\.(\w{2,4})$/, '') || '';
        const folderName = video.title?.replace(/\/?[^/]+$/, '') || '';
        return Object.assign(video, {
            fileName: fileName,
            folderName: folderName,
            ...this.parseFilename(video.title.toString() || '')
        });
    }

    private mostProbableSubtitleVideos = (subtitle: ISubtitleAttributes, parsedVideos: IFileAttributes[]): IFileAttributes[] => {
        const subTitle = (subtitle.title || subtitle.path)?.split('/')?.pop()?.replace(/\.(\w{2,4})$/, '') || '';
        const parsedSub = this.parsePath(subtitle.title || subtitle.path);
        const byFileName = parsedVideos.filter(video => subTitle.includes(video.title!));
        if (byFileName.length === 1) {
            return byFileName.map(v => v);
        }
        const byTitleSeasonEpisode = parsedVideos.filter(video => video.title === parsedSub.title
            && parsedSub.seasons && parsedSub.episodes
            && this.arrayEquals(video.seasons || [], parsedSub.seasons)
            && this.arrayEquals(video.episodes || [], parsedSub.episodes));
        if (this.singleVideoFile(byTitleSeasonEpisode)) {
            return byTitleSeasonEpisode.map(v => v);
        }
        const bySeasonEpisode = parsedVideos.filter(video => parsedSub.seasons && parsedSub.episodes
            && this.arrayEquals(video.seasons || [], parsedSub.seasons)
            && this.arrayEquals(video.episodes || [], parsedSub.episodes));
        if (this.singleVideoFile(bySeasonEpisode)) {
            return bySeasonEpisode.map(v => v);
        }
        const byTitle = parsedVideos.filter(video => video.title && video.title === parsedSub.title);
        if (this.singleVideoFile(byTitle)) {
            return byTitle.map(v => v);
        }
        const byEpisode = parsedVideos.filter(video => parsedSub.episodes
            && this.arrayEquals(video.episodes || [], parsedSub.episodes || []));
        if (this.singleVideoFile(byEpisode)) {
            return byEpisode.map(v => v);
        }
        const byInfoHash = parsedVideos.filter(video => video.infoHash === subtitle.infoHash);
        if (this.singleVideoFile(byInfoHash)) {
            return byInfoHash.map(v => v);
        }
        return [];
    }

    private singleVideoFile = (videos: IFileAttributes[]): boolean => {
        return new Set(videos.map(v => v.fileIndex)).size === 1;
    }

    private parsePath = (path: string): IFileAttributes => {
        const pathParts = path.split('/').map(part => this.parseFilename(part));
        const parsedWithEpisode = pathParts.find(parsed => parsed.season && parsed.episodes);
        return parsedWithEpisode || pathParts[pathParts.length - 1];
    }

    private parseFilename = (filename: string): IFileAttributes => {
        const parsedInfo = parse(filename)
        const titleEpisode = parsedInfo.title.match(/(\d+)$/);
        if (!parsedInfo.episodes && titleEpisode) {
            parsedInfo.episodes = [parseInt(titleEpisode[1], 10)];
        }
        return parsedInfo;
    }

    private arrayEquals = <T>(array1: T[], array2: T[]): boolean => {
        if (!array1 || !array2) return array1 === array2;
        return array1.length === array2.length && array1.every((value, index) => value === array2[index])
    }
}