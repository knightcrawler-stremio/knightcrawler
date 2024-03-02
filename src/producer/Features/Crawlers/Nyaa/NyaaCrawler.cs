namespace Producer.Features.Crawlers.Nyaa;

public class NyaaCrawler(IHttpClientFactory httpClientFactory, ILogger<NyaaCrawler> logger, IDataStorage storage) : BaseXmlCrawler(httpClientFactory, logger, storage)
{
    protected override string Url => "https://nyaa.si/?page=rss&c=1_2&f=0";
    protected override string Source => "Nyaa";
    
    private static readonly XNamespace XmlNamespace = "https://nyaa.si/xmlns/nyaa"; 

    protected override IReadOnlyDictionary<string, string> Mappings =>
        new Dictionary<string, string>
        {
            [nameof(Torrent.Name)] = "title",
            [nameof(Torrent.Size)] = "size",
            [nameof(Torrent.Seeders)] = "seeders",
            [nameof(Torrent.Leechers)] = "leechers",
            [nameof(Torrent.InfoHash)] = "infoHash",
            [nameof(Torrent.Category)] = "category",
        };

    protected override Torrent ParseTorrent(XElement itemNode) =>
        new()
        {
            Source = Source,
            Name = itemNode.Element(Mappings[nameof(Torrent.Name)])?.Value,
            Size = itemNode.Element(XmlNamespace + Mappings[nameof(Torrent.Size)])?.Value,
            Seeders = int.Parse(itemNode.Element(XmlNamespace + Mappings[nameof(Torrent.Seeders)])?.Value ?? "0"),
            Leechers = int.Parse(itemNode.Element(XmlNamespace + Mappings[nameof(Torrent.Leechers)])?.Value ?? "0"),
            InfoHash = itemNode.Element(XmlNamespace + Mappings[nameof(Torrent.InfoHash)])?.Value,
            Category = itemNode.Element(Mappings[nameof(Torrent.Category)])?.Value.ToLowerInvariant(),
        };
}