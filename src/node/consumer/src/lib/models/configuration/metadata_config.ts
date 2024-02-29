export const metadataConfig = {
    IMDB_CONCURRENT: parseInt(process.env.IMDB_CONCURRENT || "1", 10),
    IMDB_INTERVAL_MS: parseInt(process.env.IMDB_INTERVAL_MS || "1000", 10),
    TITLE_MATCH_THRESHOLD: Number(process.env.TITLE_MATCH_THRESHOLD || 0.25),
};