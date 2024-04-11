namespace Producer.Features.Crawlers.Yts;

public class YtsCrawler(IHttpClientFactory httpClientFactory, ILogger<YtsCrawler> logger, IDataStorage storage, ScrapeConfiguration scrapeConfiguration) : BaseXmlCrawler(httpClientFactory, logger, storage)
{
    protected override string Url => scrapeConfiguration.Scrapers.FirstOrDefault(x => x.Name.Equals("SyncYtsJob", StringComparison.OrdinalIgnoreCase))?.Url ?? string.Empty;
    protected override string Source => "YTS";
    protected override IReadOnlyDictionary<string, string> Mappings
        => new Dictionary<string, string>
        {
            [nameof(IngestedTorrent.Name)] = "title",
            [nameof(IngestedTorrent.Size)] = "size",
            [nameof(IngestedTorrent.Seeders)] = "seeders",
            [nameof(IngestedTorrent.Leechers)] = "leechers",
            [nameof(IngestedTorrent.InfoHash)] = "enclosure",
        };

    protected override IngestedTorrent? ParseTorrent(XElement itemNode)
    {
        var torrent = new IngestedTorrent
        {
            Source = Source,
            Name = itemNode.Element(Mappings["Name"])?.Value,
            Category = "movies",
            Size = "0",
            Seeders = 0,
            Leechers = 0,
        };

        HandleInfoHash(itemNode, torrent, "InfoHash");

        return torrent;
    }

    protected override void HandleInfoHash(XElement itemNode, IngestedTorrent torrent, string infoHashKey)
    {
        var infoHash = itemNode.Element(Mappings[infoHashKey])?.Attribute("url")?.Value.Split("/download/").ElementAtOrDefault(1);

        if (infoHash is not null)
        {
            torrent.InfoHash = infoHash;
        }
    }
}
