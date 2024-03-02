namespace Producer.Features.Crawlers.Torrentio;

public static class TorrentioInstancesExtensions
{
    public static TimeSpan CalculateWaitTime(this TorrentioInstance instance, Dictionary<string, TorrentioScrapeInstance> scraperState)
    {
        if (!scraperState.TryGetValue(instance.Name, out var state))
        {
            state = new (DateTime.UtcNow, 0, 0, null);
            scraperState[instance.Name] = state;
        }

        var (startedAt, requestCount, totalProcessed, lastProcessedImdbId) = state;
        
        if (requestCount < instance.RateLimit.RequestLimit)
        {
            scraperState[instance.Name] = new (startedAt, requestCount + 1, totalProcessed + 1, lastProcessedImdbId);
            return TimeSpan.Zero;
        }

        var elapsed = DateTime.UtcNow - startedAt;
        var interval = TimeSpan.FromSeconds(instance.RateLimit.IntervalInSeconds);
        var remaining = interval - elapsed;

        // reset the state for the next interval
        scraperState[instance.Name] = new (DateTime.UtcNow, 0, totalProcessed, lastProcessedImdbId);

        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }
    
    public static void SetPossiblyRateLimited(this TorrentioInstance instance, Dictionary<string, TorrentioScrapeInstance> scraperState, int minutesToWait = 5)
    {
        if (!scraperState.TryGetValue(instance.Name, out var state))
        {
            state = new (DateTime.UtcNow, 0, 0, null);
        }

        var (_, _, totalProcessed, lastProcessedImdbId) = state;

        // Set the start time to 15 minutes in the past so that the next check will result in a rate limit period of 15 minutes
        var startedAt = DateTime.UtcNow.AddMinutes(-minutesToWait);
        var requestCount = instance.RateLimit.RequestLimit;

        // Update the scraper state for the instance
        scraperState[instance.Name] = new (startedAt, requestCount, totalProcessed, lastProcessedImdbId);
    }
    
    public static long TotalProcessedRequests(this TorrentioInstance instance, Dictionary<string, TorrentioScrapeInstance> scraperState) => 
        !scraperState.TryGetValue(instance.Name, out var state) ? 0 : state.TotalProcessed;
    
    public static string? LastProcessedImdbId(this TorrentioInstance instance, Dictionary<string, TorrentioScrapeInstance> scraperState) => 
        !scraperState.TryGetValue(instance.Name, out var state) ? null : state.LastProcessedImdbId;
}