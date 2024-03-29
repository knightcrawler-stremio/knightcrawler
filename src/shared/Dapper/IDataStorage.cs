namespace SharedContracts.Dapper;

public interface IDataStorage
{
    Task<DapperResult<InsertTorrentResult, InsertTorrentResult>> InsertTorrents(IReadOnlyCollection<IngestedTorrent> torrents, CancellationToken cancellationToken = default);
    Task<DapperResult<List<IngestedTorrent>, List<IngestedTorrent>>> GetPublishableTorrents(CancellationToken cancellationToken = default);
    Task<DapperResult<UpdatedTorrentResult, UpdatedTorrentResult>> SetTorrentsProcessed(IReadOnlyCollection<IngestedTorrent> torrents, CancellationToken cancellationToken = default);
    Task<bool> PageIngested(string pageId, CancellationToken cancellationToken = default);
    Task<DapperResult<PageIngestedResult, PageIngestedResult>> MarkPageAsIngested(string pageId, CancellationToken cancellationToken = default);
    Task<DapperResult<int, int>> GetRowCountImdbMetadata(CancellationToken cancellationToken = default);
    Task<List<ImdbEntry>> GetImdbEntriesForRequests(int year, int batchSize, string? stateLastProcessedImdbId, CancellationToken cancellationToken = default);
    Task<ImdbEntry?> FindImdbMetadata(string? parsedTorrentTitle, string parsedTorrentTorrentType, int? parsedTorrentYear, CancellationToken cancellationToken = default);
    Task InsertTorrent(Torrent torrent, CancellationToken cancellationToken = default);
    Task InsertFiles(IEnumerable<TorrentFile> files, CancellationToken cancellationToken = default);
    Task InsertSubtitles(IEnumerable<SubtitleFile> subtitles, CancellationToken cancellationToken = default);
    Task<List<TorrentFile>> GetTorrentFiles(string infoHash, CancellationToken cancellationToken = default);
}
