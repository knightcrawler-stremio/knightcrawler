namespace Tissue.Features.DataProcessing;

public interface IDataStorage
{
    Task<IReadOnlyCollection<Torrent>?> GetAllTorrents(CancellationToken cancellationToken = default);
    Task DeleteTorrentsByInfoHashes(IReadOnlyCollection<string> infoHashes, CancellationToken cancellationToken = default);
}
