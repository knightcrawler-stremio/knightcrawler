import {Optional} from "sequelize";
import {SubtitleAttributes} from "./subtitle_attributes";
import {ParseTorrentTitleResult} from "../../lib/interfaces/parse_torrent_title_result";

export interface FileAttributes extends ParseTorrentTitleResult {
    id?: number;
    infoHash?: string;
    fileIndex?: number;
    title?: string;
    size?: number;
    imdbId?: string;
    imdbSeason?: number;
    imdbEpisode?: number;
    kitsuId?: number;
    kitsuEpisode?: number;
    subtitles?: SubtitleAttributes[];
    path?: string;
    isMovie?: boolean;
}

export interface FileCreationAttributes extends Optional<FileAttributes, 'fileIndex' | 'size' | 'imdbId' | 'imdbSeason' | 'imdbEpisode' | 'kitsuId' | 'kitsuEpisode'> {
}