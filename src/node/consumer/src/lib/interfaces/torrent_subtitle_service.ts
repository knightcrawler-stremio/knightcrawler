import {ITorrentFileCollection} from "./torrent_file_collection";

export interface ITorrentSubtitleService {
    assignSubtitles(fileCollection: ITorrentFileCollection): ITorrentFileCollection;
}