import {CommonVideoMetadata} from "./common_video_metadata";

export interface MetadataResponse {
    kitsuId?: number;
    imdbId?: number;
    type?: string;
    title?: string;
    year?: number;
    country?: string;
    genres?: string[];
    status?: string;
    videos?: CommonVideoMetadata[];
    episodeCount?: number[];
    totalCount?: number;
}