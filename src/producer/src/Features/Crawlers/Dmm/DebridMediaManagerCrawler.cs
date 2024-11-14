namespace Producer.Features.Crawlers.Dmm;

public partial class DebridMediaManagerCrawler(
    IDMMFileDownloader dmmFileDownloader,
    ILogger<DebridMediaManagerCrawler> logger,
    IDataStorage storage,
    IRankTorrentName rankTorrentName,
    IDistributedCache cache) : BaseCrawler(logger, storage)
{
    [GeneratedRegex("""<iframe src="https:\/\/debridmediamanager.com\/hashlist#(.*)"></iframe>""")]
    private static partial Regex HashCollectionMatcher();
    protected override string Url => "";
    protected override IReadOnlyDictionary<string, string> Mappings => new Dictionary<string, string>();
    protected override string Source => "DMM";

    private const int ParallelismCount = 4;

    public override async Task Execute()
    {
        var tempDirectory = await dmmFileDownloader.DownloadFileToTempPath(CancellationToken.None);

        var files = Directory.GetFiles(tempDirectory, "*.html", SearchOption.AllDirectories);

        logger.LogInformation("Found {Files} files to parse", files.Length);

        var options = new ParallelOptions { MaxDegreeOfParallelism = ParallelismCount };

        await Parallel.ForEachAsync(files, options, async (file, token) =>
        {
            var fileName = Path.GetFileName(file);
            var torrentDictionary = await ExtractPageContents(file, fileName);

            if (torrentDictionary == null)
            {
                return;
            }

            await ParseTitlesWithRtn(fileName, torrentDictionary);
            var results = await ParseTorrents(torrentDictionary);

            if (results.Count <= 0)
            {
                return;
            }

            await InsertTorrents(results);
            await Storage.MarkPageAsIngested(fileName, token);
        });
    }

    private async Task ParseTitlesWithRtn(string fileName, IDictionary<string, DmmContent> page)
    {
        logger.LogInformation("Parsing titles for {Page}", fileName);

        var batchProcessables = page.Select(value => new RtnBatchProcessable(value.Key, value.Value.Filename)).ToList();
        var parsedResponses = rankTorrentName.BatchParse(
            batchProcessables.Select<RtnBatchProcessable, string>(bp => bp.Filename).ToList(), trashGarbage: false);

        // Filter out unsuccessful responses and match RawTitle to requesting title
        var successfulResponses = parsedResponses
            .Where(response => response != null && response.Success)
            .GroupBy(response => response.Response.RawTitle!)
            .ToDictionary(group => group.Key, group => group.First());

        var options = new ParallelOptions { MaxDegreeOfParallelism = ParallelismCount };

        await Parallel.ForEachAsync(batchProcessables.Select(t => t.InfoHash), options, (infoHash, _) =>
        {
            if (page.TryGetValue(infoHash, out var dmmContent) &&
                successfulResponses.TryGetValue(dmmContent.Filename, out var parsedResponse))
            {
                page[infoHash] = dmmContent with { ParseResponse = parsedResponse };
            }

            return ValueTask.CompletedTask;
        });
    }

    private async Task<ConcurrentDictionary<string, DmmContent>?> ExtractPageContents(string filePath, string filenameOnly)
    {
        var (pageIngested, name) = await IsAlreadyIngested(filenameOnly);

        if (pageIngested)
        {
            return [];
        }

        var pageSource = await File.ReadAllTextAsync(filePath);

        var match = HashCollectionMatcher().Match(pageSource);

        if (!match.Success)
        {
            logger.LogWarning("Failed to match hash collection for {Name}", name);
            await Storage.MarkPageAsIngested(filenameOnly);
            return [];
        }

        var encodedJson = match.Groups.Values.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(encodedJson?.Value))
        {
            logger.LogWarning("Failed to extract encoded json for {Name}", name);
            return [];
        }

        var decodedJson = LZString.DecompressFromEncodedURIComponent(encodedJson.Value);

        JsonElement arrayToProcess;
        try
        {
            var json = JsonDocument.Parse(decodedJson);

            if (json.RootElement.ValueKind == JsonValueKind.Object &&
                json.RootElement.TryGetProperty("torrents", out var torrentsProperty) &&
                torrentsProperty.ValueKind == JsonValueKind.Array)
            {
                arrayToProcess = torrentsProperty;
            }
            else if (json.RootElement.ValueKind == JsonValueKind.Array)
            {
                arrayToProcess = json.RootElement;
            }
            else
            {
                logger.LogWarning("Unexpected JSON format in {Name}", name);
                return [];
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to parse JSON {decodedJson} for {Name}: {Exception}", decodedJson, name, ex);
            return [];
        }

        var torrents = await arrayToProcess.EnumerateArray()
            .ToAsyncEnumerable()
            .Select(ParsePageContent)
            .Where(t => t is not null)
            .ToListAsync();

        if (torrents.Count == 0)
        {
            logger.LogWarning("No torrents found in {Name}", name);
            await Storage.MarkPageAsIngested(filenameOnly);
            return [];
        }

        var torrentDictionary = torrents
            .Where(x => x is not null)
            .GroupBy(x => x.InfoHash)
            .ToConcurrentDictionary(g => g.Key, g => new DmmContent(g.First().Filename, g.First().Bytes, null));

        logger.LogInformation("Parsed {Torrents} torrents for {Name}", torrentDictionary.Count, name);

        return torrentDictionary;
    }

    private async Task<List<IngestedTorrent>> ParseTorrents(IDictionary<string, DmmContent> page)
    {
        var ingestedTorrents = new List<IngestedTorrent>();

        var options = new ParallelOptions { MaxDegreeOfParallelism = ParallelismCount };

        await Parallel.ForEachAsync(page, options, async (kvp, ct) =>
        {
            var (infoHash, dmmContent) = kvp;
            var parsedTorrent = dmmContent.ParseResponse;
            if (parsedTorrent is not { Success: true })
            {
                return;
            }

            var torrentType = parsedTorrent.Response.IsMovie ? "movie" : "tvSeries";
            var cacheKey = GetCacheKey(torrentType, parsedTorrent.Response.ParsedTitle, parsedTorrent.Response.Year);
            var (cached, cachedResult) = await CheckIfInCacheAndReturn(cacheKey);

            if (cached)
            {
                logger.LogInformation("[{ImdbId}] Found cached imdb result for {Title}", cachedResult.ImdbId, parsedTorrent.Response.ParsedTitle);
                lock (ingestedTorrents)
                {
                    ingestedTorrents.Add(MapToTorrent(cachedResult, dmmContent.Bytes, infoHash, parsedTorrent));
                }
                return;
            }

            int? year = parsedTorrent.Response.Year != 0 ? parsedTorrent.Response.Year : null;
            var imdbEntry = await Storage.FindImdbMetadata(parsedTorrent.Response.ParsedTitle, torrentType, year, ct);

            if (imdbEntry is null)
            {
                return;
            }

            await AddToCache(cacheKey, imdbEntry);
            logger.LogInformation("[{ImdbId}] Found best match for {Title}: {BestMatch} with score {Score}", imdbEntry.ImdbId, parsedTorrent.Response.ParsedTitle, imdbEntry.Title, imdbEntry.Score);
            lock (ingestedTorrents)
            {
                ingestedTorrents.Add(MapToTorrent(imdbEntry, dmmContent.Bytes, infoHash, parsedTorrent));
            }
        });

        return ingestedTorrents;
    }

    private IngestedTorrent MapToTorrent(ImdbEntry result, long size, string infoHash, ParseTorrentTitleResponse parsedTorrent) =>
        new()
        {
            Source = Source,
            Name = result.Title,
            Imdb = result.ImdbId,
            Size = size.ToString(),
            InfoHash = infoHash,
            Seeders = 0,
            Leechers = 0,
            Category = AssignCategory(result),
            RtnResponse = parsedTorrent.Response.ToJson(),
        };


    private Task AddToCache(string cacheKey, ImdbEntry best)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
        };

        return cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(best), cacheOptions);
    }

    private async Task<(bool Success, ImdbEntry? Entry)> CheckIfInCacheAndReturn(string cacheKey)
    {
        var cachedImdbId = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedImdbId))
        {
            return (true, JsonSerializer.Deserialize<ImdbEntry>(cachedImdbId));
        }

        return (false, null);
    }

    private async Task<(bool Success, string? Name)> IsAlreadyIngested(string filename)
    {
        var pageIngested = await Storage.PageIngested(filename);

        return (pageIngested, filename);
    }

    private static string AssignCategory(ImdbEntry entry) =>
        entry.Category.ToLower() switch
        {
            var category when string.Equals(category, "movie", StringComparison.OrdinalIgnoreCase) => "movies",
            var category when string.Equals(category, "tvSeries", StringComparison.OrdinalIgnoreCase) => "tv",
            _ => "unknown",
        };

    private static string GetCacheKey(string category, string title, int year) => $"{category.ToLowerInvariant()}:{year}:{title.ToLowerInvariant()}";

    private static ExtractedDMMContent? ParsePageContent(JsonElement item)
    {
        if (!item.TryGetProperty("filename", out var filenameElement) ||
            !item.TryGetProperty("bytes", out var bytesElement) ||
            !item.TryGetProperty("hash", out var hashElement))
        {
            return null;
        }

        return new(filenameElement.GetString(), bytesElement.GetInt64(), hashElement.GetString());
    }

    private record DmmContent(string Filename, long Bytes, ParseTorrentTitleResponse? ParseResponse);
    private record ExtractedDMMContent(string Filename, long Bytes, string InfoHash);
    private record RtnBatchProcessable(string InfoHash, string Filename);
}
