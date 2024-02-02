namespace Producer.Interfaces;

public interface IMessagePublisher
{
    Task<bool> PublishAsync(IReadOnlyCollection<Torrent> torrents, CancellationToken cancellationToken = default);
}