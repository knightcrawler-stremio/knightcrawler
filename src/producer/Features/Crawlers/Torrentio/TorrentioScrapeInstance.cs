namespace Producer.Features.Crawlers.Torrentio;

public record TorrentioScrapeInstance(DateTime StartedAt, int RequestCount, int TotalProcessed, string? LastProcessedImdbId);