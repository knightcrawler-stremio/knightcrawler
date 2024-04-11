namespace Producer.Features.Crawlers.EzTv;

public class EzTvCrawler(IHttpClientFactory httpClientFactory, ILogger<EzTvCrawler> logger, IDataStorage storage, ScrapeConfiguration scrapeConfiguration) : BaseXmlCrawler(httpClientFactory, logger, storage)
{
    protected override string Url => scrapeConfiguration.Scrapers.FirstOrDefault(x => x.Name.Equals("SyncEzTvJob", StringComparison.OrdinalIgnoreCase))?.Url ?? string.Empty;
    protected override string Source => "EZTV";
    private XNamespace XmlNamespace => scrapeConfiguration.Scrapers.FirstOrDefault(x => x.Name.Equals("SyncEzTvJob", StringComparison.OrdinalIgnoreCase))?.XmlNamespace ?? string.Empty;

    protected override IReadOnlyDictionary<string, string> Mappings =>
        new Dictionary<string, string>
        {
            [nameof(IngestedTorrent.Name)] = "title",
            [nameof(IngestedTorrent.Size)] = "contentLength",
            [nameof(IngestedTorrent.Seeders)] = "seeds",
            [nameof(IngestedTorrent.Leechers)] = "peers",
            [nameof(IngestedTorrent.InfoHash)] = "infoHash",
            [nameof(IngestedTorrent.Category)] = "category",
        };

    protected override IngestedTorrent ParseTorrent(XElement itemNode) =>
        new()
        {
            Source = Source,
            Name = itemNode.Element(Mappings[nameof(IngestedTorrent.Name)])?.Value,
            Size = itemNode.Element(XmlNamespace + Mappings[nameof(IngestedTorrent.Size)])?.Value,
            Seeders = int.Parse(itemNode.Element(XmlNamespace + Mappings[nameof(IngestedTorrent.Seeders)])?.Value ?? "0"),
            Leechers = int.Parse(itemNode.Element(XmlNamespace + Mappings[nameof(IngestedTorrent.Leechers)])?.Value ?? "0"),
            InfoHash = itemNode.Element(XmlNamespace + Mappings[nameof(IngestedTorrent.InfoHash)])?.Value,
            Category = itemNode.Element(Mappings[nameof(IngestedTorrent.Category)])?.Value.ToLowerInvariant(),
        };
}
