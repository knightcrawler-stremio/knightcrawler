namespace Tissue.Features.Kleenex;

public class KleenexService(IDataStorage dataStorage, ILogger<KleenexService> logger, IWordCollections wordCollections) : IHostedService
{
    private HashSet<string> _combinedCompounds = [];

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Kleenex service started");
            logger.LogInformation("Get ready to pop all that corn...");

            var torrents = await LoadTorrents(cancellationToken);

            _combinedCompounds.UnionWith(wordCollections.AdultCompoundPhrases);
            _combinedCompounds.UnionWith(wordCollections.Jav);
            _combinedCompounds.UnionWith(wordCollections.AdultStars);

            var infoHashesToDelete = GetInfoHashesToDelete(torrents);

            await dataStorage.DeleteTorrentsByInfoHashes(infoHashesToDelete!, cancellationToken);

            logger.LogInformation("Deleted {TorrentCount} torrents", infoHashesToDelete.Count);

            logger.LogInformation("Kleenex service completed successfully");

            Environment.Exit(0);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while processing torrents");
            Environment.Exit(1);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Service Shutdown");
        return Task.CompletedTask;
    }

    private List<string?> GetInfoHashesToDelete(IReadOnlyCollection<Torrent> torrents)
    {
        var torrentsToDelete = torrents.Where(torrent => HasBannedTerms(torrent.Title)).ToList();
        var infoHashesToDelete = torrentsToDelete.Select(torrent => torrent.InfoHash).ToList();

        if (infoHashesToDelete.Count == 0)
        {
            logger.LogInformation("No torrents to delete");
            Environment.Exit(0);
        }

        return infoHashesToDelete;
    }

    private async Task<IReadOnlyCollection<Torrent>> LoadTorrents(CancellationToken cancellationToken)
    {
        var torrents = await dataStorage.GetAllTorrents(cancellationToken);

        if (torrents is null || torrents.Count == 0)
        {
            logger.LogInformation("No torrents found");
            Environment.Exit(0);
        }

        logger.LogInformation("Found {TorrentCount} torrents", torrents.Count);
        return torrents;
    }

    private bool HasBannedTerms(string targetTitle)
    {
        var normalisedTitle = targetTitle.NormalizeTitle();

        var normalisedWords = normalisedTitle.Split(' ');

        var hasBannedWords = normalisedWords.Where(word => word.Length >= 3).Any(word => normalisedWords.Contains(word, StringComparer.OrdinalIgnoreCase) && wordCollections.AdultWords.Contains(word));

        var hasCompounds = _combinedCompounds.Any(term => normalisedTitle.Contains(term, StringComparison.OrdinalIgnoreCase));

        var isClean = !hasBannedWords &&
                      !hasCompounds;

        if (isClean)
        {
            logger.LogInformation("No banned terms found in torrent title: {Title}", targetTitle);
            return false;
        }

        logger.LogWarning("Banned terms found in torrent title: {Title}", targetTitle);
        return true;
    }
}
