export interface IMongoMetadataQuery {
    $text: { $search: string },
    TitleType: string,
    StartYear?: string;
}
