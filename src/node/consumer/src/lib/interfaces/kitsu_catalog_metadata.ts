import {KitsuLink, KitsuTrailer} from "./kitsu_metadata";

export interface KitsuCatalogJsonResponse {
    metas: KitsuCatalogMetaData[];
}

export interface KitsuCatalogMetaData {
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
    trailers: KitsuTrailer[];
    links: KitsuLink[];
}