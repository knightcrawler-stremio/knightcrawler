namespace DebridCollector.Features.Debrid;

public class DebridRequestProcessor(IDebridHttpClient debridHttpClient, ILogger<DebridRequestProcessor> logger, IBus messageBus) : BackgroundService
{
    private const int BatchDelay = 3000;
    public const int MaxBatchSize = 100;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var requests = new List<PerformMetadataRequest>(MaxBatchSize);
        var delay = TimeSpan.FromMilliseconds(BatchDelay);
        while (!stoppingToken.IsCancellationRequested)
        {
            while (ProcessorChannel.Queue.Reader.TryRead(out var request))
            {
                if (requests.Count >= MaxBatchSize)
                {
                    break;
                }
                
                if (requests.All(x => x.InfoHash != request.InfoHash))
                {
                    requests.Add(request);
                }
            }

            if (requests.Any())
            {
                await ProcessRequests(requests, stoppingToken);
                requests.Clear();
            }
            
            await Task.Delay(delay, stoppingToken);
        }

        // After the loop ends, there may be remaining requests which were not processed. Let's process them:
        if (requests.Count != 0)
        {
            await ProcessRequests(requests, stoppingToken);
            requests.Clear();
        }
    }
    
    private async Task ProcessRequests(IReadOnlyCollection<PerformMetadataRequest> requests, CancellationToken stoppingToken = default)
    {
        try
        {
            var results = await debridHttpClient.GetMetadataAsync(requests, stoppingToken);
            await ProcessResponses(results);
            logger.LogInformation("Processed: {Count} infoHashes", requests.Count);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to process infoHashes");
        }
    }
        
    private async Task ProcessResponses(IEnumerable<TorrentMetadataResponse> results)
    {
        var messages = results.Select(response => new GotMetadata(response)).ToList();

        await messageBus.PublishBatch(messages);
    }
}