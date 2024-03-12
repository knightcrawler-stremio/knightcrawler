namespace Tissue.Features.Wordlists;

public class PopulationService(IWordCollections wordCollections, ILogger<PopulationService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Loading word collections...");

        await wordCollections.LoadAsync();

        logger.LogInformation("Adult Words Count: {Count}", wordCollections.AdultWords.Count);
        logger.LogInformation("Adult Compound Phrases Count: {Count}", wordCollections.AdultCompoundPhrases.Count);
        logger.LogInformation("Jav Count: {Count}", wordCollections.Jav.Count);
        logger.LogInformation("Adult Stars Count: {Count}", wordCollections.AdultStars.Count);

        logger.LogInformation("Word collections loaded.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
