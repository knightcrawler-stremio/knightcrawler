namespace SharedContracts.Dapper;

public class DapperDataStorage(PostgresConfiguration configuration, RabbitMqConfiguration rabbitConfig, ILogger<DapperDataStorage> logger) : 
    BaseDapperStorage(logger, configuration), IDataStorage
{
    public async Task<DapperResult<InsertTorrentResult, InsertTorrentResult>> InsertTorrents(IReadOnlyCollection<IngestedTorrent> torrents, CancellationToken cancellationToken = default) =>
        await ExecuteCommandAsync(async connection =>
        {
            const string query = 
                """
                INSERT INTO ingested_torrents
                    ("name", "source", "category", "info_hash", "size", "seeders", "leechers", "imdb", "processed", "createdAt", "updatedAt")
                VALUES
                    (@Name, @Source, @Category, @InfoHash, @Size, @Seeders, @Leechers, @Imdb, @Processed, @CreatedAt, @UpdatedAt)
                ON CONFLICT (source, info_hash) DO NOTHING
                """;
            
            var inserted = await connection.ExecuteAsync(query, torrents);
            return new InsertTorrentResult(true, inserted);
        }, _ => new InsertTorrentResult(false, 0, "Failed to insert torrents."), cancellationToken);

    public async Task<DapperResult<List<IngestedTorrent>, List<IngestedTorrent>>> GetPublishableTorrents(CancellationToken cancellationToken = default) =>
        await ExecuteCommandAsync(async connection =>
        {
            const string query =
                """
                SELECT
                    "id" as "Id",
                    "name" as "Name",
                    "source" as "Source",
                    "category" as "Category",
                    "info_hash" as "InfoHash",
                    "size" as "Size",
                    "seeders" as "Seeders",
                    "leechers" as "Leechers",
                    "imdb" as "Imdb",
                    "processed" as "Processed",
                    "createdAt" as "CreatedAt",
                    "updatedAt" as "UpdatedAt"
                FROM ingested_torrents
                WHERE processed = false AND category != 'xxx'
                """;
            
            var torrents = await connection.QueryAsync<IngestedTorrent>(query);
            return torrents.Take(rabbitConfig.MaxPublishBatchSize).ToList();
        }, _ => new List<IngestedTorrent>(), cancellationToken);

    public async Task<DapperResult<UpdatedTorrentResult, UpdatedTorrentResult>> SetTorrentsProcessed(IReadOnlyCollection<IngestedTorrent> torrents, CancellationToken cancellationToken = default) =>
        await ExecuteCommandAsync(async connection =>
        {
            foreach (var torrent in torrents)
            {
                torrent.UpdatedAt = DateTime.UtcNow;
            }

            const string query =
                """
                UPDATE ingested_torrents
                Set
                    processed = true,
                    "updatedAt" = @UpdatedAt
                WHERE id = @Id
                """;
            
            var updated = await connection.ExecuteAsync(query, torrents);
            return new UpdatedTorrentResult(true, updated);
        }, _ => new UpdatedTorrentResult(false, 0, "Failed to mark torrents as processed"), cancellationToken);

    public async Task<bool> PageIngested(string pageId, CancellationToken cancellationToken = default) =>
        await ExecuteCommandAsync(async connection =>
        {
            const string query = "SELECT EXISTS (SELECT 1 FROM ingested_pages WHERE url = @Url)";
            return await connection.ExecuteScalarAsync<bool>(query, new { Url = pageId });
        }, "Failed to check if page is ingested", cancellationToken);

    public async Task<DapperResult<PageIngestedResult, PageIngestedResult>> MarkPageAsIngested(string pageId, CancellationToken cancellationToken = default) =>
        await ExecuteCommandAsync(async connection =>
        {
            var date = DateTime.UtcNow;

            const string query =
                """
                INSERT INTO ingested_pages
                    (url, "createdAt", "updatedAt")
                VALUES
                    (@Url, @CreatedAt, @UpdatedAt)
                """;
            
            await connection.ExecuteAsync(query, new
            {
                Url = pageId,
                CreatedAt = date,
                UpdatedAt = date,
            });

            return new PageIngestedResult(true, "Page successfully marked as ingested");
            
        }, _ => new PageIngestedResult(false, "Page successfully marked as ingested"), cancellationToken);

    public async Task<DapperResult<int, int>> GetRowCountImdbMetadata(CancellationToken cancellationToken = default) =>
        await ExecuteCommandAsync(async connection =>
        {
            const string query = "SELECT COUNT(*) FROM imdb_metadata";

            var result = await connection.ExecuteScalarAsync<int>(query);

            return result;
        }, _ => 0, cancellationToken);

    public async Task<List<ImdbEntry>> GetImdbEntriesForRequests(int year, int batchSize, string? stateLastProcessedImdbId, CancellationToken cancellationToken = default) =>
        await ExecuteCommandAsync(async connection =>
        {
            const string query = @"SELECT imdb_id AS ImdbId, title as Title, category as Category, year as Year, adult as Adult FROM imdb_metadata WHERE Year <= @Year AND imdb_id > @LastProcessedImdbId ORDER BY ImdbId LIMIT @BatchSize";
            var result = await connection.QueryAsync<ImdbEntry>(query, new { Year = year, LastProcessedImdbId = stateLastProcessedImdbId, BatchSize = batchSize });
            return result.ToList();
        }, "Error getting imdb metadata.", cancellationToken);

    public async Task<ImdbEntry?> FindImdbMetadata(string? parsedTorrentTitle, string torrentType, int? year, CancellationToken cancellationToken = default) =>
        await ExecuteCommandAsync(async connection =>
        {
            var query = $"select \"imdb_id\" as \"ImdbId\", \"title\" as \"Title\", \"year\" as \"Year\", \"score\" as Score from search_imdb_meta('{parsedTorrentTitle.Replace("'", "").Replace("\"", "")}', '{(torrentType.Equals("movie", StringComparison.OrdinalIgnoreCase) ? "movie" : "tvSeries")}'";
            query += year is not null ? $", {year}" : ", NULL";
            query += ", 1)";
            
            var result = await connection.QueryAsync<ImdbEntry>(query);
            var results = result.ToList();
            return results.FirstOrDefault();
        }, "Error finding imdb metadata.", cancellationToken);
    
    public Task InsertTorrent(Torrent torrent, CancellationToken cancellationToken = default) =>
        ExecuteCommandAsync(
            async connection =>
            {
                const string query = 
                    """
                    INSERT INTO "torrents"
                        ("infoHash", "provider", "torrentId", "title", "size", "type", "uploadDate", "seeders", "trackers", "languages", "resolution", "reviewed", "opened", "createdAt", "updatedAt")
                    VALUES
                        (@InfoHash, @Provider, @TorrentId, @Title, 0, @Type, NOW(), @Seeders, NULL, NULL, NULL, false, false, NOW(), NOW())
                    ON CONFLICT ("infoHash") DO NOTHING
                    """;
                
                await connection.ExecuteAsync(query, torrent);
            }, "Failed to insert torrent files into database", cancellationToken);
    
    public Task InsertFiles(IEnumerable<TorrentFile> files, CancellationToken cancellationToken = default) =>
        ExecuteCommandAsync(
            async connection =>
            {
                const string query = 
                    """
                    INSERT INTO files
                        ("infoHash", "fileIndex", title, "size", "imdbId", "imdbSeason", "imdbEpisode", "kitsuId", "kitsuEpisode", "createdAt", "updatedAt")
                    VALUES
                        (@InfoHash, @FileIndex, @Title, @Size, @ImdbId, @ImdbSeason, @ImdbEpisode, @KitsuId, @KitsuEpisode, Now(), Now());
                    """;
                
                await connection.ExecuteAsync(query, files);
            }, "Failed to insert torrent files into database", cancellationToken);
    
    public Task InsertSubtitles(IEnumerable<SubtitleFile> subtitles, CancellationToken cancellationToken = default) =>
        ExecuteCommandAsync(
            async connection =>
            {
                const string query = 
                    """
                    INSERT INTO subtitles
                        ("infoHash", "fileIndex", "fileId", "title")
                    VALUES
                        (@InfoHash, @FileIndex, @FileId, @Title)
                    ON CONFLICT
                        ("infoHash", "fileIndex")
                    DO UPDATE SET
                        "fileId" = COALESCE(subtitles."fileId", EXCLUDED."fileId"),
                        "title" = COALESCE(subtitles."title", EXCLUDED."title");
                    """;
                
                await connection.ExecuteAsync(query, subtitles);
            }, "Failed to insert subtitles into database", cancellationToken);

    public Task<List<TorrentFile>> GetTorrentFiles(string infoHash, CancellationToken cancellationToken = default) =>
        ExecuteCommandAsync(
            async connection =>
            {
                const string query = "SELECT * FROM files WHERE LOWER(\"infoHash\") = @InfoHash";
                var files = await connection.QueryAsync<TorrentFile>(query, new { InfoHash = infoHash });
                return files.ToList();
            }, "Failed to insert subtitles into database", cancellationToken);
}
