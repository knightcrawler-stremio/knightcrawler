namespace Scraper.Services;

public class TorrentPublisher(ISendEndpointProvider sendEndpointProvider, RabbitMqConfiguration configuration) : IMessagePublisher
{
    public async Task PublishAsync(IEnumerable<Torrent> torrents, CancellationToken cancellationToken = default)
    {
        var queueAddress = ConstructQueue();
        var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new(queueAddress));
        await sendEndpoint.SendBatch(torrents, cancellationToken: cancellationToken);
    }
    
    private string ConstructQueue()
    {
        var queueBuilder = new StringBuilder();
        queueBuilder.Append("queue:");
        queueBuilder.Append(configuration.QueueName);
        queueBuilder.Append("?durable=");
        queueBuilder.Append(configuration.Durable ? "true" : "false");

        return queueBuilder.ToString();
    }
}