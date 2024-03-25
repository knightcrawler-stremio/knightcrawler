namespace QBitCollector.Features.Trackers;

public class TrackersService(IDistributedCache cache, HttpClient client, IMemoryCache memoryCache) : ITrackersService
{
    private const string TrackersListUrl = "https://ngosang.github.io/trackerslist/trackers_all.txt";
    private const string CacheKey = "trackers";

    public async Task<List<string>> GetTrackers()
    {
        if (memoryCache.TryGetValue(CacheKey, out List<string> memoryCachedTrackers))
        {
            return memoryCachedTrackers;
        }
        
        var cachedTrackers = await cache.GetStringAsync(CacheKey);

        if (!string.IsNullOrWhiteSpace(cachedTrackers))
        {
            var trackersList = JsonSerializer.Deserialize<List<string>>(cachedTrackers);
            memoryCache.Set(CacheKey, trackersList, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4),
            });
            
            return trackersList;
        }

        var trackers = await GetTrackersAsync();
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2),
        };
        await cache.SetStringAsync(CacheKey, JsonSerializer.Serialize(trackers), cacheOptions);
        
        memoryCache.Set(CacheKey, trackers, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4),
        });
        
        return trackers;
    }

    private async Task<List<string>> GetTrackersAsync()
    {
        var response = await client.GetStringAsync(TrackersListUrl);

        var lines = response.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

        var nonEmptyLines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

        return nonEmptyLines;
    }

    
}