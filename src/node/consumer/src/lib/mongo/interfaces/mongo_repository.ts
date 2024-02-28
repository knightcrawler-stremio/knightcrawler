export interface IMongoRepository {
    connect(): Promise<void>;
    getImdbId(title: string, category: string, year?: string | number): Promise<string | null>;
}