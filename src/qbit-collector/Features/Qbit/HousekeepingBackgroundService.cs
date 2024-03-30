namespace QBitCollector.Features.Qbit;

public class HousekeepingBackgroundService(IQBittorrentClient client, ILogger<HousekeepingBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Service is Running.");

        await DoWork();

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(2));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWork();
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Service stopping.");
        }
    }

    private async Task DoWork()
    {
        try
        {
            logger.LogInformation("Cleaning Stale Entries in Qbit...");

            var torrents = await client.GetTorrentListAsync();

            foreach (var torrentInfo in torrents)
            {
                if (!(torrentInfo.AddedOn < DateTimeOffset.UtcNow.AddMinutes(-1)))
                {
                    continue;
                }
                
                logger.LogInformation("Torrent [{InfoHash}] Identified as stale because was added at {AddedOn}", torrentInfo.Hash, torrentInfo.AddedOn);

                await client.DeleteAsync(new[] {torrentInfo.Hash}, deleteDownloadedData: true);
                logger.LogInformation("Cleaned up stale torrent: [{InfoHash}]", torrentInfo.Hash);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error cleaning up stale torrents this interval.");
        }
    }
}