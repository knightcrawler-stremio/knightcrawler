import {ContentAttributes} from "../../repository/interfaces/content_attributes";
import {FileAttributes} from "../../repository/interfaces/file_attributes";
import {SubtitleAttributes} from "../../repository/interfaces/subtitle_attributes";

export interface TorrentFileCollection {
    contents?: ContentAttributes[];
    videos?: FileAttributes[];
    subtitles?: SubtitleAttributes[];
}