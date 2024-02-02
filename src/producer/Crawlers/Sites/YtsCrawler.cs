namespace Producer.Crawlers.Sites;

public class YtsCrawler(IHttpClientFactory httpClientFactory, ILogger<YtsCrawler> logger, IDataStorage storage) : BaseXmlCrawler(httpClientFactory, logger, storage)
{
    protected override string Url => "https://yts.am/rss";

    protected override string Source => "YTS";
    protected override IReadOnlyDictionary<string, string> Mappings
        => new Dictionary<string, string>
        {
            [nameof(Torrent.Name)] = "title",
            [nameof(Torrent.Size)] = "size",
            [nameof(Torrent.Seeders)] = "seeders",
            [nameof(Torrent.Leechers)] = "leechers",
            [nameof(Torrent.InfoHash)] = "enclosure",
        };

    protected override Torrent? ParseTorrent(XElement itemNode)
    {
        var torrent = new Torrent
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

    protected override void HandleInfoHash(XElement itemNode, Torrent torrent, string infoHashKey)
    {
        var infoHash = itemNode.Element(Mappings[infoHashKey])?.Attribute("url")?.Value.Split("/download/").ElementAtOrDefault(1);
        
        if (infoHash is not null)
        {
            torrent.InfoHash = infoHash;
        }
    }
}