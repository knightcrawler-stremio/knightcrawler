namespace Producer.Services;

public class DapperDataStorage(ScrapeConfiguration configuration, RabbitMqConfiguration rabbitConfig, ILogger<DapperDataStorage> logger) : IDataStorage
{
    private const string InsertTorrentSql =
        """
        INSERT INTO ingested_torrents (name, source, category, info_hash, size, seeders, leechers, imdb, processed, "createdAt", "updatedAt")
        VALUES (@Name, @Source, @Category, @InfoHash, @Size, @Seeders, @Leechers, @Imdb, @Processed, @CreatedAt, @UpdatedAt)
        ON CONFLICT (source, info_hash) DO NOTHING
        """;
    
    private const string InsertIngestedPageSql =
        """
        INSERT INTO ingested_pages (url, "createdAt", "updatedAt")
        VALUES (@Url, @CreatedAt, @UpdatedAt)
        """;
    
    private const string GetMovieAndSeriesTorrentsNotProcessedSql =
        """
        SELECT 
            id as "Id",
            name as "Name",
            source as "Source",
            category as "Category",
            info_hash as "InfoHash",
            size as "Size",
            seeders as "Seeders",
            leechers as "Leechers",
            imdb as "Imdb",
            processed as "Processed",
            "createdAt" as "CreatedAt",
            "updatedAt" as "UpdatedAt" 
        FROM ingested_torrents
        WHERE processed = false AND category != 'xxx'
        """;
    
    private const string UpdateProcessedSql =
        """
        UPDATE ingested_torrents
        Set 
            processed = true, 
            "updatedAt" = @UpdatedAt
        WHERE id = @Id
        """;
    
    public async Task<InsertTorrentResult> InsertTorrents(IReadOnlyCollection<Torrent> torrents, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(configuration.StorageConnectionString);
            await connection.OpenAsync(cancellationToken);
            var inserted = await connection.ExecuteAsync(InsertTorrentSql, torrents);
            return new(true, inserted);
        }
        catch (Exception e)
        {
            return new(false, 0, e.Message);
        }
    }

    public async Task<IReadOnlyCollection<Torrent>> GetPublishableTorrents(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(configuration.StorageConnectionString);
            await connection.OpenAsync(cancellationToken);
            var torrents = await connection.QueryAsync<Torrent>(GetMovieAndSeriesTorrentsNotProcessedSql);
            return torrents.Take(rabbitConfig.MaxPublishBatchSize).ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while getting publishable torrents from database");
            return new List<Torrent>();
        }
    }
    
    public async Task<UpdatedTorrentResult> SetTorrentsProcessed(IReadOnlyCollection<Torrent> torrents, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(configuration.StorageConnectionString);
            await connection.OpenAsync(cancellationToken);
            
            foreach (var torrent in torrents)
            {
                torrent.UpdatedAt = DateTime.UtcNow;
            }
            
            var updated = await connection.ExecuteAsync(UpdateProcessedSql, torrents);
            return new(true, updated);
        }
        catch (Exception e)
        {
            return new(false, 0, e.Message);
        }
    }

    public async Task<bool> PageIngested(string pageId, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(configuration.StorageConnectionString);
        await connection.OpenAsync(cancellationToken);
        
        const string query = "SELECT EXISTS (SELECT 1 FROM ingested_pages WHERE url = @Url)";
        
        var result = await connection.ExecuteScalarAsync<bool>(query, new { Url = pageId });
        
        return result;
    }

    public async Task<PageIngestedResult> MarkPageAsIngested(string pageId, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(configuration.StorageConnectionString);
        await connection.OpenAsync(cancellationToken);

        try
        {
            var date = DateTime.UtcNow;
            
            await connection.ExecuteAsync(InsertIngestedPageSql, new
            {
                Url = pageId,
                CreatedAt = date,
                UpdatedAt = date,
            });
            
            return new(true, "Page successfully marked as ingested");
        }
        catch (Exception e)
        {
            return new(false, $"Failed to mark page as ingested: {e.Message}");
        }
    }
}