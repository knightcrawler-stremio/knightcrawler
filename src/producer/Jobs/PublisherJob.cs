namespace Producer.Jobs;

[DisallowConcurrentExecution]
public class PublisherJob(IMessagePublisher publisher, IDataStorage storage, ILogger<PublisherJob> logger) : IJob
{
    private const string JobName = nameof(PublisherJob);
    public static readonly JobKey Key = new(JobName, nameof(Jobs));
    public static readonly TriggerKey Trigger = new($"{JobName}-trigger", nameof(Jobs));
    
    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context.CancellationToken;
        var torrents = await storage.GetPublishableTorrents(cancellationToken);
        
        if (torrents.Count == 0)
        {
            return;
        }
        
        await publisher.PublishAsync(torrents, cancellationToken);
        var result = await storage.SetTorrentsProcessed(torrents, cancellationToken);
        
        if (!result.Success)
        {
            logger.LogWarning("Failed to set torrents as processed: [{Error}]", result.ErrorMessage);
            return;
        }
        
        logger.LogInformation("Successfully set {Count} torrents as processed", result.UpdatedCount);
    }
}