using System.Collections;

namespace Producer.Features.DataProcessing;

public interface IDataStorage
{
    Task<InsertTorrentResult> InsertTorrents(IReadOnlyCollection<Torrent> torrents, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Torrent>> GetPublishableTorrents(CancellationToken cancellationToken = default);
    Task<UpdatedTorrentResult> SetTorrentsProcessed(IReadOnlyCollection<Torrent> torrents, CancellationToken cancellationToken = default);
    Task<bool> PageIngested(string pageId, CancellationToken cancellationToken = default);
    Task<PageIngestedResult> MarkPageAsIngested(string pageId, CancellationToken cancellationToken = default);
    Task<int> GetRowCountImdbMetadata(CancellationToken cancellationToken = default);
    Task<List<ImdbEntry>> GetImdbEntriesForRequests(int year, int batchSize, string? stateLastProcessedImdbId, CancellationToken cancellationToken = default);
}
