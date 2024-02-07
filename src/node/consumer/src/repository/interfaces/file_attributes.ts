import {Optional} from "sequelize";
import {ISubtitleAttributes} from "./subtitle_attributes";
import {IParseTorrentTitleResult} from "../../lib/interfaces/parse_torrent_title_result";

export interface IFileAttributes extends IParseTorrentTitleResult {
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
    subtitles?: ISubtitleAttributes[];
    path?: string;
    isMovie?: boolean;
}

export interface IFileCreationAttributes extends Optional<IFileAttributes, 'fileIndex' | 'size' | 'imdbId' | 'imdbSeason' | 'imdbEpisode' | 'kitsuId' | 'kitsuEpisode'> {
}