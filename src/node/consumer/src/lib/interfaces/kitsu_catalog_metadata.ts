import {IKitsuLink, IKitsuTrailer} from "@interfaces/kitsu_metadata";

export interface IKitsuCatalogJsonResponse {
    metas: IKitsuCatalogMetaData[];
}

export interface IKitsuCatalogMetaData {
    id: string;
    type: string;
    animeType: string;
    name: string;
    aliases: string[];
    description: string;
    releaseInfo: string;
    runtime: string;
    imdbRating: string;
    genres: string[];
    logo?: string;
    poster: string;
    background: string;
    trailers: IKitsuTrailer[];
    links: IKitsuLink[];
}