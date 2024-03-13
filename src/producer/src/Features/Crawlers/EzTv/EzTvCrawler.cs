namespace Producer.Features.Crawlers.EzTv;

public class EzTvCrawler(IHttpClientFactory httpClientFactory, ILogger<EzTvCrawler> logger, IDataStorage storage) : BaseXmlCrawler(httpClientFactory, logger, storage)
{
    protected override string Url => "https://eztv1.xyz/ezrss.xml";
    protected override string Source => "EZTV";

    private static readonly XNamespace XmlNamespace = "http://xmlns.ezrss.it/0.1/";

    protected override IReadOnlyDictionary<string, string> Mappings =>
        new Dictionary<string, string>
        {
            [nameof(Torrent.Name)] = "title",
            [nameof(Torrent.Size)] = "contentLength",
            [nameof(Torrent.Seeders)] = "seeds",
            [nameof(Torrent.Leechers)] = "peers",
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
