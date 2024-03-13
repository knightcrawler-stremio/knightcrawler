namespace Producer.Features.Crawlers.Torrentio;

public class TorrentioRateLimit
{
    public int RequestLimit { get; set; }
    public int IntervalInSeconds { get; set; }

    public int BatchSize { get; set; }

    public int ExceptionLimit { get; set; }

    public int ExceptionIntervalInSeconds { get; set; }
}
