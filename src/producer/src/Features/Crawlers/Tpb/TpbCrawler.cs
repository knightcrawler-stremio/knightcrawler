namespace Producer.Features.Crawlers.Tpb;

public class TpbCrawler(IHttpClientFactory httpClientFactory, ILogger<TpbCrawler> logger, IDataStorage storage, ScrapeConfiguration scrapeConfiguration) : BaseJsonCrawler(httpClientFactory, logger, storage)
{
    protected override string Url => scrapeConfiguration.Scrapers.FirstOrDefault(x => x.Name.Equals("SyncTpbJob", StringComparison.OrdinalIgnoreCase))?.Url ?? string.Empty;

    protected override string Source => "TPB";
    
    // // ReSharper disable once UnusedMember.Local
    // private readonly Dictionary<string, Dictionary<string, int>> TpbCategories = new()
    // {
    //     {
    //         "VIDEO", new()
    //         {
    //             {"ALL", 200},
    //             {"MOVIES", 201},
    //             {"MOVIES_DVDR", 202},
    //             {"MUSIC_VIDEOS", 203},
    //             {"MOVIE_CLIPS", 204},
    //             {"TV_SHOWS", 205},
    //             {"HANDHELD", 206},
    //             {"MOVIES_HD", 207},
    //             {"TV_SHOWS_HD", 208},
    //             {"MOVIES_3D", 209},
    //             {"OTHER", 299},
    //         }
    //     },
    //     {
    //         "PORN", new()
    //         {
    //             {"ALL", 500},
    //             {"MOVIES", 501},
    //             {"MOVIES_DVDR", 502},
    //             {"PICTURES", 503},
    //             {"GAMES", 504},
    //             {"MOVIES_HD", 505},
    //             {"MOVIE_CLIPS", 506},
    //             {"OTHER", 599},
    //         }
    //     },
    // };
    
    private static readonly HashSet<int> TvSeriesCategories = [ 205, 208 ];
    private static readonly HashSet<int> MovieCategories = [ 201, 202, 207, 209 ];
    private static readonly HashSet<int> AllowedCategories = [ ..MovieCategories, ..TvSeriesCategories ];

    protected override IReadOnlyDictionary<string, string> Mappings
        => new Dictionary<string, string>
        {
            [nameof(IngestedTorrent.Name)] = "name",
            [nameof(IngestedTorrent.Size)] = "size",
            [nameof(IngestedTorrent.Seeders)] = "seeders",
            [nameof(IngestedTorrent.Leechers)] = "leechers",
            [nameof(IngestedTorrent.InfoHash)] = "info_hash",
            [nameof(IngestedTorrent.Category)] = "category",
        };

    protected override IngestedTorrent? ParseTorrent(JsonElement item)
    {
        var incomingCategory = item.GetProperty(Mappings["Category"]).GetInt32();

        if (!AllowedCategories.Contains(incomingCategory))
        {
            return null;
        }

        var torrent = new IngestedTorrent
        {
            Source = Source,
            Name = item.GetProperty(Mappings["Name"]).GetString(),
            Size = item.GetProperty(Mappings["Size"]).GetInt64().ToString(),
            Seeders = item.GetProperty(Mappings["Seeders"]).GetInt32(),
            Leechers = item.GetProperty(Mappings["Leechers"]).GetInt32(),
        };

        HandleInfoHash(item, torrent, "InfoHash");

        torrent.Category = HandleCategory(incomingCategory);

        return torrent;
    }

    private static string HandleCategory(int category) =>
        MovieCategories.Contains(category) switch
        {
            true => "movies",
            _ => TvSeriesCategories.Contains(category) switch
            {
                true => "tv",
                _ => "xxx",
            },
        };

    public override Task Execute() => Execute("items");
}
