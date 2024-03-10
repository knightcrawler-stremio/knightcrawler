namespace Producer.Features.Wordlists;

public class PopulationService(IWordCollections wordCollections, ILogger<PopulationService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Loading word collections...");

        await wordCollections.LoadAsync();

        logger.LogInformation("Common Words Count: {Count}", wordCollections.CommonWords.Count);
        logger.LogInformation("Adult Words Count: {Count}", wordCollections.AdultWords.Count);
        logger.LogInformation("Adult Compound Phrases Count: {Count}", wordCollections.AdultCompoundPhrases.Count);

        logger.LogInformation("Word collections loaded.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
