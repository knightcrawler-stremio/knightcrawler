namespace Producer.Features.Crawlers.Torrentio;

public class TorrentioInstance
{
    public string Name { get; init; } = default!;

    public string Url { get; init; } = default!;

    public TorrentioRateLimit RateLimit { get; init; } = default!;
}
