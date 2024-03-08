namespace Producer.Features.Crawlers.Torrentio;

public class TorrentioConfiguration
{
    public const string SectionName = "TorrentioConfiguration";
    public const string Filename = "torrentio.json";

    public List<TorrentioInstance> Instances { get; set; } = [];
}
