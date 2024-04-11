namespace Producer.Features.Crawlers.Tgx;

public partial class TgxCrawler(IHttpClientFactory httpClientFactory, ILogger<TgxCrawler> logger, IDataStorage storage, ScrapeConfiguration scrapeConfiguration) : BaseXmlCrawler(httpClientFactory, logger, storage)
{
    [GeneratedRegex(@"Size:\s+(.+?)\s+Added")]
    private static partial Regex SizeStringExtractor();
    [GeneratedRegex(@"(?i)\b(\d+(\.\d+)?)\s*([KMGT]?B)\b", RegexOptions.None, "en-GB")]
    private static partial Regex SizeStringParser();

    protected override string Url => scrapeConfiguration.Scrapers.FirstOrDefault(x => x.Name.Equals("SyncTgxJob", StringComparison.OrdinalIgnoreCase))?.Url ?? string.Empty;

    protected override string Source => "TorrentGalaxy";
    protected override IReadOnlyDictionary<string, string> Mappings
        => new Dictionary<string, string>
        {
            [nameof(IngestedTorrent.Name)] = "title",
            [nameof(IngestedTorrent.Size)] = "description",
            [nameof(IngestedTorrent.InfoHash)] = "guid",
            [nameof(IngestedTorrent.Category)] = "category",
        };

    private static readonly HashSet<string> AllowedCategories =
    [
        "movies",
        "tv",
    ];

    protected override IngestedTorrent? ParseTorrent(XElement itemNode)
    {
        var category = itemNode.Element(Mappings["Category"])?.Value.ToLowerInvariant();

        if (category is null)
        {
            return null;
        }

        if (!IsAllowedCategory(category))
        {
            return null;
        }


        var torrent = new IngestedTorrent
        {
            Source = Source,
            Name = itemNode.Element(Mappings["Name"])?.Value,
            InfoHash = itemNode.Element(Mappings[nameof(IngestedTorrent.InfoHash)])?.Value,
            Size = "0",
            Seeders = 0,
            Leechers = 0,
        };

        HandleSize(itemNode, torrent, "Size");

        torrent.Category = SetCategory(category);

        return torrent;
    }

    private static string SetCategory(string category) =>
        category.Contains("tv") switch
        {
            true => "tv",
            _ => category.Contains("movies") switch
            {
                true => "movies",
                _ => "xxx",
            },
        };

    private void HandleSize(XContainer itemNode, IngestedTorrent torrent, string key)
    {
        var description = itemNode.Element(Mappings[key])?.Value;

        if (description is null)
        {
            return;
        }

        var size = ExtractSizeFromDescription(description);

        if (size is not null)
        {
            torrent.Size = size.ToString();
        }
    }

    private long? ExtractSizeFromDescription(string input)
    {
        var sizeMatch = SizeStringExtractor().Match(input);

        if (!sizeMatch.Success)
        {
            throw new FormatException("Unable to parse size from the input.");
        }

        var sizeString = sizeMatch.Groups[1].Value;

        var units = new Dictionary<string, long>
        {
            { "B", 1 },
            { "KB", 1L << 10 },
            { "MB", 1L << 20 },
            { "GB", 1L << 30 },
            { "TB", 1L << 40 },
        };

        var match = SizeStringParser().Match(sizeString);

        if (match.Success)
        {
            var val = double.Parse(match.Groups[1].Value);
            var unit = match.Groups[3].Value.ToUpper();

            if (units.TryGetValue(unit, out var multiplier))
            {
                try
                {
                    var bytes = checked((long)(val * multiplier));
                    return bytes;
                }
                catch (OverflowException)
                {
                    logger.LogWarning("The size '{Size}' is too large.", sizeString);
                    return null;
                }
            }

            logger.LogWarning("The size unit '{Unit}' is not supported.", unit);
            return null;
        }

        logger.LogWarning("The size '{Size}' is not in a supported format.", sizeString);
        return null;
    }

    private static bool IsAllowedCategory(string category)
    {
        var parsedCategory = category.Split(':').ElementAtOrDefault(0)?.Trim().ToLower();

        return parsedCategory is not null && AllowedCategories.Contains(parsedCategory);
    }
}
