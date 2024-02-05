import {CommonVideoMetadata} from "./common_video_metadata";

export interface KitsuJsonResponse {
    cacheMaxAge?: number;
    meta?: KitsuMeta;
}
export interface KitsuMeta {
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
    links?: KitsuLink[];
    logo?: string;
    name?: string;
    poster?: string;
    releaseInfo?: string;
    runtime?: string;
    slug?: string;
    status?: string;
    trailers?: KitsuTrailer[];
    type?: string;
    userCount?: number;
    videos?: KitsuVideo[];
    year?: string;
}
export interface KitsuVideo extends CommonVideoMetadata {
    imdbEpisode?: number;
    imdbSeason?: number;
    imdb_id?: string;
    thumbnail?: string;
}
export interface KitsuTrailer {
    source?: string;
    type?: string;
}
export interface KitsuLink {
    name?: string;
    category?: string;
    url?: string;
}