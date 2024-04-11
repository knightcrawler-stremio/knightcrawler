namespace Producer.Features.Crawlers.Nyaa;

public class NyaaCrawler(IHttpClientFactory httpClientFactory, ILogger<NyaaCrawler> logger, IDataStorage storage, ScrapeConfiguration scrapeConfiguration) : BaseXmlCrawler(httpClientFactory, logger, storage)
{
    protected override string Url => scrapeConfiguration.Scrapers.FirstOrDefault(x => x.Name.Equals("SyncNyaaJob", StringComparison.OrdinalIgnoreCase))?.Url ?? string.Empty;
    protected override string Source => "Nyaa";
    private XNamespace XmlNamespace => scrapeConfiguration.Scrapers.FirstOrDefault(x => x.Name.Equals("SyncNyaaJob", StringComparison.OrdinalIgnoreCase))?.XmlNamespace ?? string.Empty;

    protected override IReadOnlyDictionary<string, string> Mappings =>
        new Dictionary<string, string>
        {
            [nameof(IngestedTorrent.Name)] = "title",
            [nameof(IngestedTorrent.Size)] = "size",
            [nameof(IngestedTorrent.Seeders)] = "seeders",
            [nameof(IngestedTorrent.Leechers)] = "leechers",
            [nameof(IngestedTorrent.InfoHash)] = "infoHash",
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
            Category = "anime",
        };
}
