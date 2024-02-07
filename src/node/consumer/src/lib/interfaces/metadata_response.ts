import {ICommonVideoMetadata} from "./common_video_metadata";

export interface IMetadataResponse {
    kitsuId?: number;
    imdbId?: number;
    type?: string;
    title?: string;
    year?: number;
    country?: string;
    genres?: string[];
    status?: string;
    videos?: ICommonVideoMetadata[];
    episodeCount?: number[];
    totalCount?: number;
}