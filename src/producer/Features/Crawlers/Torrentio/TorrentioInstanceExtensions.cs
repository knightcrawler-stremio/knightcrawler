namespace Producer.Features.Crawlers.Torrentio;

public static class TorrentioInstancesExtensions
{
    public static TimeSpan CalculateWaitTime(this TorrentioInstance instance, TorrentioScrapeInstance state)
    {
        if (state.RequestCount < instance.RateLimit.RequestLimit)
        {
            state.RequestCount++;
            state.TotalProcessed++;
            return TimeSpan.Zero;
        }

        var elapsed = DateTime.UtcNow - state.StartedAt;
        var interval = TimeSpan.FromSeconds(instance.RateLimit.IntervalInSeconds);
        var remaining = interval - elapsed;

        // reset the state for the next interval
        state.StartedAt = DateTime.UtcNow;
        state.RequestCount = 0;

        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    public static long TotalProcessedRequests(this TorrentioInstance instance, Dictionary<string, TorrentioScrapeInstance> scraperState) => 
        !scraperState.TryGetValue(instance.Name, out var state) ? 0 : state.TotalProcessed;
    
    public static string? LastProcessedImdbId(this TorrentioInstance instance, Dictionary<string, TorrentioScrapeInstance> scraperState) => 
        !scraperState.TryGetValue(instance.Name, out var state) ? null : state.LastProcessedImdbId;
    
    public static TorrentioScrapeInstance EnsureStateExists(this TorrentioInstance instance, Dictionary<string, TorrentioScrapeInstance> scraperState)
    {
        if (!scraperState.TryGetValue(instance.Name, out var state))
        {
            state = new();
            scraperState[instance.Name] = state;
        }

        return state;
    }
}