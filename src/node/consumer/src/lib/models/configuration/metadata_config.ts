export class MetadataConfig {
    public IMDB_CONCURRENT: number = parseInt(process.env.IMDB_CONCURRENT || "1", 10);
    public IMDB_INTERVAL_MS: number = parseInt(process.env.IMDB_INTERVAL_MS || "1000", 10);
}