namespace Producer.Features.CrawlerSupport;

public abstract class BaseJsonCrawler(IHttpClientFactory httpClientFactory, ILogger<BaseJsonCrawler> logger, IDataStorage storage) : BaseCrawler(logger, storage)
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Literals.CrawlerClient);

    protected virtual async Task Execute(string collectionName)
    {
        if (string.IsNullOrWhiteSpace(Url))
        {
            logger.LogWarning("No URL provided for {Source} crawl", Source);
            return;
        }
        
        logger.LogInformation("Starting {Source} crawl", Source);

        using var client = httpClientFactory.CreateClient("Scraper");
        var jsonBody = await _client.GetStringAsync(Url);
        using var json =  JsonDocument.Parse(jsonBody);

        var torrents = json.RootElement.EnumerateArray()
            .Select(ParseTorrent)
            .Where(x => x is not null)
            .ToList();

        if (torrents.Count == 0)
        {
            logger.LogWarning("No torrents found in {Source} response", Source);
            return;
        }

        await InsertTorrents(torrents!);
    }

    protected virtual void HandleInfoHash(JsonElement item, IngestedTorrent torrent, string infoHashKey)
    {
        if (!Mappings.ContainsKey(infoHashKey))
        {
            return;
        }

        var infoHash = item.GetProperty(Mappings[infoHashKey]).GetString();

        if (infoHash is not null)
        {
            torrent.InfoHash = infoHash;
        }
    }

    protected abstract IngestedTorrent? ParseTorrent(JsonElement item);
}
