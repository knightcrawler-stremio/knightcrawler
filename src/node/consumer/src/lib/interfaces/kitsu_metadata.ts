import {ICommonVideoMetadata} from "./common_video_metadata";

export interface IKitsuJsonResponse {
    cacheMaxAge?: number;
    meta?: IKitsuMeta;
}
export interface IKitsuMeta {
    aliases?: string[];
    animeType?: string;
    background?: string;
    description?: string;
    country?: string;
    genres?: string[];
    id?: string;
    imdbRating?: string;
    imdb_id?: string;
    kitsu_id?: string;
    links?: IKitsuLink[];
    logo?: string;
    name?: string;
    poster?: string;
    releaseInfo?: string;
    runtime?: string;
    slug?: string;
    status?: string;
    trailers?: IKitsuTrailer[];
    type?: string;
    userCount?: number;
    videos?: IKitsuVideo[];
    year?: string;
}
export interface IKitsuVideo extends ICommonVideoMetadata {
    imdbEpisode?: number;
    imdbSeason?: number;
    imdb_id?: string;
    thumbnail?: string;
}
export interface IKitsuTrailer {
    source?: string;
    type?: string;
}
export interface IKitsuLink {
    name?: string;
    category?: string;
    url?: string;
}