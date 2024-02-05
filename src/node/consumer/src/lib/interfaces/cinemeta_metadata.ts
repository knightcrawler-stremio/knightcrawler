import {CommonVideoMetadata} from "./common_video_metadata";

export interface CinemetaJsonResponse {
    meta?: CinemetaMetaData;
    trailerStreams?: CinemetaTrailerStream[];
    links?: CinemetaLink[];
    behaviorHints?: CinemetaBehaviorHints;
}
export interface CinemetaMetaData {
    awards?: string;
    cast?: string[];
    country?: string;
    description?: string;
    director?: null;
    dvdRelease?: null;
    genre?: string[];
    imdbRating?: string;
    imdb_id?: string;
    name?: string;
    popularity?: number;
    poster?: string;
    released?: string;
    runtime?: string;
    status?: string;
    tvdb_id?: number;
    type?: string;
    writer?: string[];
    year?: string;
    background?: string;
    logo?: string;
    popularities?: CinemetaPopularities;
    moviedb_id?: number;
    slug?: string;
    trailers?: CinemetaTrailer[];
    id?: string;
    genres?: string[];
    releaseInfo?: string;
    videos?: CinemetaVideo[];
}
export interface CinemetaPopularities {
    PXS_TEST?: number;
    PXS?: number;
    SCM?: number;
    EXMD?: number;
    ALLIANCE?: number;
    EJD?: number;
    moviedb?: number;
    trakt?: number;
    stremio?: number;
    stremio_lib?: number;
}
export interface CinemetaTrailer {
    source?: string;
    type?: string;
}
export interface CinemetaVideo extends CommonVideoMetadata {
    name?: string;
    number?: number;
    firstAired?: string;
    tvdb_id?: number;
    rating?: string;
    overview?: string;
    thumbnail?: string;
    description?: string;
}
export interface CinemetaTrailerStream {
    title?: string;
    ytId?: string;
}
export interface CinemetaLink {
    name?: string;
    category?: string;
    url?: string;
}
export interface CinemetaBehaviorHints {
    defaultVideoId?: null;
    hasScheduledVideos?: boolean;
}