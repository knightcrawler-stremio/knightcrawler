namespace Producer.Features.CrawlerSupport;

public abstract class BaseCrawler(ILogger<BaseCrawler> logger, IDataStorage storage) : ICrawler
{
    protected abstract IReadOnlyDictionary<string, string> Mappings { get; }
    protected abstract string Url { get; }
    protected abstract string Source { get; }
    protected IDataStorage Storage => storage;

    public virtual Task Execute() => Task.CompletedTask;

    protected async Task<InsertTorrentResult> InsertTorrents(IReadOnlyCollection<IngestedTorrent> torrent)
    {
        var result = await storage.InsertTorrents(torrent);

        if (!result.IsSuccess)
        {
            logger.LogWarning("Ingestion Failed: [{Error}]", result.Failure.ErrorMessage);
            return result.Failure;
        }

        logger.LogInformation("Ingestion Successful - Wrote {Count} new torrents", result.Success.InsertedCount);
        return result.Success;
    }
}
