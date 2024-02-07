import {IMetaDataQuery} from "./metadata_query";
import {IMetadataResponse} from "./metadata_response";

export interface IMetadataService {
    getKitsuId(info: IMetaDataQuery): Promise<string | Error>;

    getImdbId(info: IMetaDataQuery): Promise<string | undefined>;

    getMetadata(query: IMetaDataQuery): Promise<IMetadataResponse | Error>;

    isEpisodeImdbId(imdbId: string | undefined): Promise<boolean>;

    escapeTitle(title: string): string;
}