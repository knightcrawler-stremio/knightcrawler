namespace Tissue.Features.DataProcessing;

public class DapperDataStorage(PostgresConfiguration configuration, ILogger<DapperDataStorage> logger) : IDataStorage
{
    public async Task<IReadOnlyCollection<Torrent>?> GetAllTorrents(CancellationToken cancellationToken = default)
    {
        const string GetAllTorrentsSql = "SELECT * FROM torrents";

        try
        {
            await using var connection = await CreateAndOpenConnection(cancellationToken);
            var torrents = await connection.QueryAsync<Torrent>(GetAllTorrentsSql);

            return torrents.ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while torrents from database");
            return new List<Torrent>();
        }
    }

    public async Task DeleteTorrentsByInfoHashes(IReadOnlyCollection<string> infoHashes, CancellationToken cancellationToken = default)
    {
        const string deleteTorrentsSql = "DELETE FROM torrents WHERE \"infoHash\" = ANY(@infoHashes)";

        try
        {
            await using var connection = await CreateAndOpenConnection(cancellationToken);
            await connection.ExecuteAsync(deleteTorrentsSql, new { infoHashes });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while deleting torrents from database");
        }
    }

    private async Task<NpgsqlConnection> CreateAndOpenConnection(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(configuration.StorageConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
