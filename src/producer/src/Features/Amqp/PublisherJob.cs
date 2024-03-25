using Literals = Producer.Features.JobSupport.Literals;

namespace Producer.Features.Amqp;

[DisallowConcurrentExecution]
[ManualJobRegistration]
public class PublisherJob(IMessagePublisher publisher, IDataStorage storage, ILogger<PublisherJob> logger) : IJob
{
    private const string JobName = nameof(PublisherJob);
    public static readonly JobKey Key = new(JobName, nameof(Literals.PublishingJobs));
    public static readonly TriggerKey Trigger = new($"{JobName}-trigger", nameof(Literals.PublishingJobs));

    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context.CancellationToken;
        var torrents = await storage.GetPublishableTorrents(cancellationToken);

        if (!torrents.IsSuccess)
        {
            logger.LogWarning("Failed to get publishable torrents]");
            return;
        }

        if (torrents.Success.Count == 0)
        {
            return;
        }

        var published = await publisher.PublishAsync(torrents.Success, cancellationToken);

        if (!published)
        {
            return;
        }

        var result = await storage.SetTorrentsProcessed(torrents.Success, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to set torrents as processed: [{Error}]", result.Failure.ErrorMessage);
            return;
        }

        logger.LogInformation("Successfully set {Count} torrents as processed", result.Success.UpdatedCount);
    }
}
