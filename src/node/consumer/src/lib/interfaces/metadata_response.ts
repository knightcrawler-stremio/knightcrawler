export interface MetadataResponse {
    kitsuId?: number;
    imdbId?: number;
    type?: string;
    title?: string;
    year?: number;
    country?: string;
    genres?: string[];
    status?: string;
    videos?: any[];
    episodeCount?: number[];
    totalCount?: number;
}