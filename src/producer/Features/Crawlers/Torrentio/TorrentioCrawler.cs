using Literals = Producer.Features.CrawlerSupport.Literals;

namespace Producer.Features.Crawlers.Torrentio;

public partial class TorrentioCrawler(
    IHttpClientFactory httpClientFactory,
    ILogger<TorrentioCrawler> logger,
    IDataStorage storage,
    TorrentioConfiguration configuration,
    ImdbMongoDbService imdbDataService) : BaseCrawler(logger, storage)
{
    [GeneratedRegex(@"(\d+(\.\d+)?) (GB|MB)")]
    private static partial Regex SizeMatcher();
    
    private const string MovieSlug = "movie/{0}.json";
    protected override string Url => "sort=size%7Cqualityfilter=other,scr,cam,unknown/stream/{0}";
    protected override IReadOnlyDictionary<string, string> Mappings { get; } = new Dictionary<string, string>();
    protected override string Source  => "Torrentio";
    private readonly Dictionary<string, TorrentioScrapeInstance> _instanceStates = [];
    public override async Task Execute()
    {
        var client = httpClientFactory.CreateClient(Literals.CrawlerClient);
        var instances = configuration.Instances;
        var totalRecordCount = await imdbDataService.GetTotalCountAsync();
        logger.LogInformation("Total IMDB records to process: {TotalRecordCount}", totalRecordCount);
        var tasks = instances.Select(x => ProcessForInstanceAsync(x, client, totalRecordCount)).ToArray();
        await Task.WhenAll(tasks);
    }

    private Task ProcessForInstanceAsync(TorrentioInstance instance, HttpClient client, long totalRecordCount) =>
        Task.Run(
            async () =>
            {
                while (instance.TotalProcessedRequests(_instanceStates) < totalRecordCount)
                {
                    logger.LogInformation("Processing {TorrentioInstance}", instance.Name);
                    logger.LogInformation("Current processed requests: {ProcessedRequests}", instance.TotalProcessedRequests(_instanceStates));
                    
                    var items = await imdbDataService.GetImdbEntriesForRequests(
                        DateTime.UtcNow.Year.ToString(),
                        instance.RateLimit.RequestLimit,
                        instance.LastProcessedImdbId(_instanceStates));
                    
                    if (items.Count == 0)
                    {
                        logger.LogInformation("No items to process for {TorrentioInstance}", instance.Name);
                        break;
                    }
                    
                    var newTorrents = new List<Torrent>();
                    var processedItemsCount = 0;

                    foreach (var item in items)
                    {
                        processedItemsCount++;
                        
                        var waitTime = instance.CalculateWaitTime(_instanceStates);

                        if (waitTime > TimeSpan.Zero)
                        {
                            logger.LogInformation("Rate limit reached for {TorrentioInstance}", instance.Name);
                            logger.LogInformation("Waiting for {TorrentioInstance}: {WaitTime}", instance.Name, waitTime);
                            await Task.Delay(waitTime);
                        }
                        
                        if (processedItemsCount % 2 == 0)
                        {
                            var randomWait = new Random().Next(1000, 5000);
                            logger.LogInformation("Waiting for {TorrentioInstance}: {WaitTime}", instance.Name, randomWait);
                            await Task.Delay(randomWait);
                        }
                        
                        try
                        {
                            var torrentInfo = await ScrapeInstance(instance, item.ImdbId, client);
                            if (torrentInfo is not null)
                            {
                                newTorrents.AddRange(torrentInfo.Where(x => x != null).Select(x => x!));
                            }
                        }
                        catch (Exception error)
                        {
                            logger.LogError(error, "page processing error in TorrentioCrawler");
                        }
                    }
                    
                    if (newTorrents.Count > 0)
                    {
                        await InsertTorrents(newTorrents);
                        
                        var currentState = _instanceStates[instance.Name];
                        _instanceStates[instance.Name] = currentState with
                        {
                            LastProcessedImdbId = items[^1].ImdbId,
                        };
                    }
                }
            });

    private async Task<List<Torrent?>?> ScrapeInstance(TorrentioInstance instance, string imdbId, HttpClient client)
    {
        logger.LogInformation("Searching Torrentio {TorrentioInstance}: {ImdbId}", instance.Name, imdbId);
        try
        {
            var movieSlug = string.Format(MovieSlug, imdbId);
            var urlSlug = string.Format(Url, movieSlug);
            return await RunRequest(instance, urlSlug, imdbId, client);
        }
        catch (Exception error)
        {
            logger.LogError(error, "page processing error {TorrentioInstance}: {ImdbId}", instance.Name, imdbId);
            logger.LogWarning("Setting {TorrentioInstance} to possibly rate limited for Five minutes", instance.Name);
            instance.SetPossiblyRateLimited(_instanceStates);
        }
        return null;
    }

    private async Task<List<Torrent?>?> RunRequest(TorrentioInstance instance, string urlSlug, string imdbId, HttpClient client)
    {
        var requestUrl = $"{instance.Url}/{urlSlug}";
        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to fetch {Url}", requestUrl);
            return null;
        }
        
        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var streams = json.RootElement.GetProperty("streams").EnumerateArray();
        return streams.Select(x => ParseTorrent(instance, x, imdbId)).Where(x => x != null).ToList();
    }
    
    private Torrent? ParseTorrent(TorrentioInstance instance, JsonElement item, string imdId)
    {
        var title = item.GetProperty("title").GetString();
        var infoHash = item.GetProperty("infoHash").GetString();
        
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(infoHash))
        {
            return null;
        }
        
        var torrent = ParseTorrentDetails(title, instance, infoHash, imdId);
        
        if (torrent is null || string.IsNullOrEmpty(torrent.Name))
        {
            return null;
        }
        
        return torrent;
    }

    private Torrent? ParseTorrentDetails(string title, TorrentioInstance instance, string infoHash, string imdbId)
    {
        try
        {
            var torrent = new Torrent
            {
                Source = $"{Source}_{instance.Name}",
                InfoHash = infoHash,
                Category = "movies", // we only handle movies for now...
                Imdb = imdbId,
            };

            var span = title.AsSpan();
            var titleEnd = span.IndexOf('\n');
            var titlePart = titleEnd >= 0 ? span[..titleEnd].ToString() : title;

            torrent.Name = titlePart.Replace('.', ' ').TrimEnd('.');

            var sizeMatch = SizeMatcher().Match(title);

            if (sizeMatch.Success)
            {
                var size = double.Parse(sizeMatch.Groups[1].Value); // Size Value
                var sizeUnit = sizeMatch.Groups[3].Value; // Size Unit (GB/MB)

                var sizeInBytes = sizeUnit switch
                {
                    "GB" => (long) (size * 1073741824),
                    "MB" => (long) (size * 1048576),
                    _ => 0,
                };

                torrent.Size = sizeInBytes.ToString();
            }

            return torrent;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error parsing torrent details");
            return null;
        }
    }
}