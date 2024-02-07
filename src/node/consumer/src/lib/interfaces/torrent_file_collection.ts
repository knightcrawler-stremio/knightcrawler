import {IContentAttributes} from "@repository/interfaces/content_attributes";
import {IFileAttributes} from "@repository/interfaces/file_attributes";
import {ISubtitleAttributes} from "@repository/interfaces/subtitle_attributes";

export interface ITorrentFileCollection {
    contents?: IContentAttributes[];
    videos?: IFileAttributes[];
    subtitles?: ISubtitleAttributes[];
}