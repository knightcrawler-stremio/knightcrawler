namespace QBitCollector.Features.Qbit;

public class QbitRequestProcessor(IQBittorrentClient client, ITrackersService trackersService, ILogger<QbitRequestProcessor> logger, QbitConfiguration configuration)
{
    public async Task<IReadOnlyList<TorrentContent>?> ProcessAsync(string infoHash, CancellationToken cancellationToken = default)
    {
        var trackers = await trackersService.GetTrackers();
        
        var magnetLink = CreateMagnetLink(infoHash, trackers);

        await client.AddTorrentsAsync(new AddTorrentUrlsRequest(new[] { new Uri(magnetLink) }), cancellationToken);

        IReadOnlyList<TorrentContent> metadata = null;

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        timeoutCts.CancelAfter(TimeSpan.FromSeconds(60));

        try
        {
            while (!timeoutCts.Token.IsCancellationRequested)
            {
                var torrentInfo = await client.GetTorrentContentsAsync(infoHash, timeoutCts.Token);

                if (torrentInfo is not null && torrentInfo.Count > 0)
                {
                    await client.DeleteAsync(new[] { infoHash }, deleteDownloadedData: true, timeoutCts.Token);
                    metadata = torrentInfo;
                    logger.LogInformation("Got metadata for torrent {InfoHash}", infoHash);
                    break;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(200), timeoutCts.Token);
            }
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            await client.DeleteAsync(new[] { infoHash }, deleteDownloadedData: true, cancellationToken);
            return null;
        }

        return metadata;
    }

    private static string CreateMagnetLink(string infoHash, List<string> trackers)
    {
        var magnetLink = $"magnet:?xt=urn:btih:{infoHash}";

        if (trackers.Count > 0)
        {
            magnetLink += $"&tr={string.Join("&tr=", trackers)}";
        }

        return magnetLink;
    }
}