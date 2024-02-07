import {IParseTorrentTitleResult} from "@interfaces/parse_torrent_title_result";
import {ISubtitleAttributes} from "@repository/interfaces/subtitle_attributes";
import {Optional} from "sequelize";

export interface IFileAttributes extends IParseTorrentTitleResult {
    id?: number;
    infoHash?: string;
    fileIndex?: number;
    title: string;
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