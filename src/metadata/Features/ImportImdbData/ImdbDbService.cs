namespace Metadata.Features.ImportImdbData;

public class ImdbDbService(PostgresConfiguration configuration, ILogger<ImdbDbService> logger)
{
    public async Task InsertImdbEntries(IEnumerable<ImdbEntry> entries)
    {
        try
        {
            await using var connection = new NpgsqlConnection(configuration.StorageConnectionString);
            await connection.OpenAsync();

            await using var writer = await connection.BeginBinaryImportAsync("COPY imdb_metadata (\"imdb_id\", \"category\", \"title\", \"year\", \"adult\") FROM STDIN (FORMAT BINARY)");

            foreach (var entry in entries)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(entry.ImdbId, NpgsqlTypes.NpgsqlDbType.Text);
                await writer.WriteAsync(entry.Category, NpgsqlTypes.NpgsqlDbType.Text);
                await writer.WriteAsync(entry.Title, NpgsqlTypes.NpgsqlDbType.Text);
                await writer.WriteAsync(entry.Year, NpgsqlTypes.NpgsqlDbType.Text);
                await writer.WriteAsync(entry.Adult, NpgsqlTypes.NpgsqlDbType.Boolean);
            }

            await writer.CompleteAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while inserting imdb entries into database");
        }
    }
}
