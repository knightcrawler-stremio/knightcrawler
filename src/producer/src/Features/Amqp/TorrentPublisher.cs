namespace Producer.Features.Amqp;

public class TorrentPublisher(
    IBus messageBus,
    RabbitMqConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    ILogger<TorrentPublisher> logger) : IMessagePublisher
{
    public async Task<bool> PublishAsync(IReadOnlyCollection<IngestedTorrent> torrents, CancellationToken cancellationToken = default)
    {
        if (!await CanPublishToRabbitMq(torrents, cancellationToken))
        {
            logger.LogWarning("Queue is full or not accessible, not publishing this time.");
            return false;
        }

        var outgoingMessages = torrents.Select(ingestedTorrent => new IngestTorrent(NewId.NextGuid(), ingestedTorrent)).ToList();

        await messageBus.PublishBatch(outgoingMessages, cancellationToken: cancellationToken);

        return true;
    }

    private async Task<bool> CanPublishToRabbitMq(IReadOnlyCollection<IngestedTorrent> torrents, CancellationToken cancellationToken)
    {
        if (configuration.MaxQueueSize == 0)
        {
            return true;
        }

        var client = httpClientFactory.CreateClient("RabbitMq");

        client.DefaultRequestHeaders.Authorization =
            new("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{configuration.Username}:{configuration.Password}")));

        var url = $"http://{configuration.Host}:15672/api/queues/{Uri.EscapeDataString("/")}/{configuration.QueueName}";

        var response = await client.GetAsync(url, cancellationToken);

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var doc = JsonDocument.Parse(body);

        if (!doc.RootElement.TryGetProperty("messages", out var messages))
        {
            logger.LogWarning("Failed to get message count from RabbitMq");
            return false;
        }

        if (!messages.TryGetInt32(out var messageCount))
        {
            logger.LogWarning("Failed to get message count from RabbitMq");
            return false;
        }

        logger.LogInformation("Current queue message count: {MessageCount}", messageCount);

        var canPublish = messageCount < configuration.MaxQueueSize + torrents.Count;

        if (!canPublish)
        {
            logger.LogWarning("You have configured a max queue size of {MaxQueueSize}.", configuration.MaxQueueSize);
            logger.LogWarning("Not publishing this time.");
        }

        return canPublish;
    }
}
