namespace Producer.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync(IEnumerable<Torrent> torrents, CancellationToken cancellationToken = default);
}