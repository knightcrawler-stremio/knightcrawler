namespace Producer.Features.Crawlers.Dmm;

public partial class DebridMediaManagerCrawler(
    IHttpClientFactory httpClientFactory,
    ILogger<DebridMediaManagerCrawler> logger,
    IDataStorage storage,
    GithubConfiguration githubConfiguration,
    IParseTorrentTitle parseTorrentTitle,
    IDistributedCache cache) : BaseCrawler(logger, storage)
{
    [GeneratedRegex("""<iframe src="https:\/\/debridmediamanager.com\/hashlist#(.*)"></iframe>""")]
    private static partial Regex HashCollectionMatcher();

    private const string DownloadBaseUrl = "https://raw.githubusercontent.com/debridmediamanager/hashlists/main";
    protected override IReadOnlyDictionary<string, string> Mappings => new Dictionary<string, string>();
    protected override string Url => "https://api.github.com/repos/debridmediamanager/hashlists/git/trees/main?recursive=1";
    protected override string Source => "DMM";
    
    public override async Task Execute()
    {
        var client = httpClientFactory.CreateClient("Scraper");
        client.DefaultRequestHeaders.Authorization = new("Bearer", githubConfiguration.PAT);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("curl");

        var jsonBody = await client.GetStringAsync(Url);

        var json =  JsonDocument.Parse(jsonBody);

        var entriesArray = json.RootElement.GetProperty("tree");

        logger.LogInformation("Found {Entries} total DMM pages", entriesArray.GetArrayLength());

        foreach (var entry in entriesArray.EnumerateArray())
        {
            await ParsePage(entry, client);
        }
    }

    private async Task ParsePage(JsonElement entry, HttpClient client)
    {
        var (pageIngested, name) = await IsAlreadyIngested(entry);

        if (string.IsNullOrEmpty(name) || pageIngested)
        {
            return;
        }

        var pageSource = await client.GetStringAsync($"{DownloadBaseUrl}/{name}");

        await ExtractPageContents(pageSource, name);
    }

    private async Task ExtractPageContents(string pageSource, string name)
    {
        var match = HashCollectionMatcher().Match(pageSource);

        if (!match.Success)
        {
            logger.LogWarning("Failed to match hash collection for {Name}", name);
            await Storage.MarkPageAsIngested(name);
            return;
        }

        var encodedJson = match.Groups.Values.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(encodedJson?.Value))
        {
            logger.LogWarning("Failed to extract encoded json for {Name}", name);
            return;
        }

        await ProcessExtractedContentsAsTorrentCollection(encodedJson.Value, name);
    }

    private async Task ProcessExtractedContentsAsTorrentCollection(string encodedJson, string name)
    {
        var decodedJson = LZString.DecompressFromEncodedURIComponent(encodedJson);

        var json = JsonDocument.Parse(decodedJson);

        await InsertTorrentsForPage(json);

        var result = await Storage.MarkPageAsIngested(name);

        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to mark page as ingested: [{Error}]", result.Failure.ErrorMessage);
            return;
        }

        logger.LogInformation("Successfully marked page as ingested");
    }

    private async Task<IngestedTorrent?> ParseTorrent(JsonElement item)
    {

        if (!item.TryGetProperty("filename", out var filenameElement) ||
            !item.TryGetProperty("bytes", out var bytesElement) ||
            !item.TryGetProperty("hash", out var hashElement))
        {
            return null;
        }

        var torrentTitle = filenameElement.GetString();

        if (torrentTitle.IsNullOrEmpty())
        {
            return null;
        }

        var parsedTorrent = parseTorrentTitle.Parse(torrentTitle.CleanTorrentTitleForImdb());
        
        var (cached, cachedResult) = await CheckIfInCacheAndReturn(parsedTorrent.Title);
        
        if (cached)
        {
            logger.LogInformation("[{ImdbId}] Found cached imdb result for {Title}", cachedResult.ImdbId, parsedTorrent.Title);
            return new()
            {
                Source = Source,
                Name = cachedResult.Title,
                Imdb = cachedResult.ImdbId,
                Size = bytesElement.GetInt64().ToString(),
                InfoHash = hashElement.ToString(),
                Seeders = 0,
                Leechers = 0,
                Category = parsedTorrent.TorrentType switch
                {
                    TorrentType.Movie => "movies",
                    TorrentType.Tv => "tv",
                    _ => "unknown",
                },
            };
        }
        
        var imdbEntry = await Storage.FindImdbMetadata(parsedTorrent.Title, parsedTorrent.TorrentType, parsedTorrent.Year);

        if (imdbEntry.Count == 0)
        {
            return null;
        }
        
        var scoredTitles = await ScoreTitles(parsedTorrent, imdbEntry);
        
        if (!scoredTitles.Success)
        {
            return null;
        }
        
        logger.LogInformation("[{ImdbId}] Found best match for {Title}: {BestMatch} with score {Score}", scoredTitles.BestMatch.Value.ImdbId, parsedTorrent.Title, scoredTitles.BestMatch.Value.Title, scoredTitles.BestMatch.Score);

        var torrent = new IngestedTorrent
        {
            Source = Source,
            Name = scoredTitles.BestMatch.Value.Title,
            Imdb = scoredTitles.BestMatch.Value.ImdbId,
            Size = bytesElement.GetInt64().ToString(),
            InfoHash = hashElement.ToString(),
            Seeders = 0,
            Leechers = 0,
            Category = parsedTorrent.TorrentType switch
            {
                TorrentType.Movie => "movies",
                TorrentType.Tv => "tv",
                _ => "unknown",
            },
        };

        return torrent;
    }

    private async Task<(bool Success, ExtractedResult<ImdbEntry>? BestMatch)> ScoreTitles(TorrentMetadata parsedTorrent, List<ImdbEntry> imdbEntries)
    {
        var lowerCaseTitle = parsedTorrent.Title.ToLowerInvariant();
       
        // Scoring directly operates on the List<ImdbEntry>, no need for lookup table.
        var scoredResults = Process.ExtractAll(new(){Title = lowerCaseTitle}, imdbEntries, x => x.Title?.ToLowerInvariant(), scorer: new DefaultRatioScorer(), cutoff: 90);

        var best = scoredResults.MaxBy(x => x.Score);

        if (best is null)
        {
            return (false, null);
        }
        
        await AddToCache(lowerCaseTitle, best);
        
        return (true, best);
    }

    private Task AddToCache(string lowerCaseTitle, ExtractedResult<ImdbEntry> best)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
        };
        
        return cache.SetStringAsync(lowerCaseTitle, JsonSerializer.Serialize(best.Value), cacheOptions);
    }

    private async Task<(bool Success, ImdbEntry? Entry)> CheckIfInCacheAndReturn(string title)
    {
        var cachedImdbId = await cache.GetStringAsync(title.ToLowerInvariant());
        
        if (!string.IsNullOrEmpty(cachedImdbId))
        {
            return (true, JsonSerializer.Deserialize<ImdbEntry>(cachedImdbId));
        }
        
        return (false, null);
    }

    private async Task InsertTorrentsForPage(JsonDocument json)
    {
        var torrents = await json.RootElement.EnumerateArray()
            .ToAsyncEnumerable()
            .SelectAwait(async x => await ParseTorrent(x))
            .Where(t => t is not null)
            .ToListAsync();

        if (torrents.Count == 0)
        {
            logger.LogWarning("No torrents found in {Source} response", Source);
            return;
        }

        await InsertTorrents(torrents!);
    }

    private async Task<(bool Success, string? Name)> IsAlreadyIngested(JsonElement entry)
    {
        var name = entry.GetProperty("path").GetString();

        if (string.IsNullOrEmpty(name))
        {
            return (false, null);
        }

        var pageIngested = await Storage.PageIngested(name);

        return (pageIngested, name);
    }
}
