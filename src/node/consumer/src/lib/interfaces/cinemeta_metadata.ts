import {ICommonVideoMetadata} from "./common_video_metadata";

export interface ICinemetaJsonResponse {
    meta?: ICinemetaMetaData;
    trailerStreams?: ICinemetaTrailerStream[];
    links?: ICinemetaLink[];
    behaviorHints?: ICinemetaBehaviorHints;
}

export interface ICinemetaMetaData {
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
    popularities?: ICinemetaPopularities;
    moviedb_id?: number;
    slug?: string;
    trailers?: ICinemetaTrailer[];
    id?: string;
    genres?: string[];
    releaseInfo?: string;
    videos?: ICinemetaVideo[];
}

export interface ICinemetaPopularities {
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

export interface ICinemetaTrailer {
    source?: string;
    type?: string;
}

export interface ICinemetaVideo extends ICommonVideoMetadata {
    name?: string;
    number?: number;
    firstAired?: string;
    tvdb_id?: number;
    rating?: string;
    overview?: string;
    thumbnail?: string;
    description?: string;
}

export interface ICinemetaTrailerStream {
    title?: string;
    ytId?: string;
}

export interface ICinemetaLink {
    name?: string;
    category?: string;
    url?: string;
}

export interface ICinemetaBehaviorHints {
    defaultVideoId?: null;
    hasScheduledVideos?: boolean;
}