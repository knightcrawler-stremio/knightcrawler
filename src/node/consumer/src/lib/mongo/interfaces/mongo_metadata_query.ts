export interface IMongoMetadataQuery {
    PrimaryTitle: { $regex: RegExp };
    TitleType: {$in: string[]};
    StartYear?: string;
}