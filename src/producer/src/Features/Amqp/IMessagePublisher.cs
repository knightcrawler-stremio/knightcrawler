namespace Producer.Features.Amqp;

public interface IMessagePublisher
{
    Task<bool> PublishAsync(IReadOnlyCollection<IngestedTorrent> torrents, CancellationToken cancellationToken = default);
}
