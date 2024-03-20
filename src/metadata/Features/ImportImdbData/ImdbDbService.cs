namespace Metadata.Features.ImportImdbData;

public class ImdbDbService(PostgresConfiguration configuration, ILogger<ImdbDbService> logger)
{
    public async Task InsertImdbEntries(IEnumerable<ImdbEntry> entries)
    {
        const string sqlCommand = "COPY imdb_metadata (\"imdb_id\", \"category\", \"title\", \"year\", \"adult\") FROM STDIN (FORMAT BINARY)";
        
        try
        {
            await using var connection = new NpgsqlConnection(configuration.StorageConnectionString);
            await connection.OpenAsync();

            await using var writer = await connection.BeginBinaryImportAsync(sqlCommand);

            foreach (var entry in entries)
            {
                try
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(entry.ImdbId, NpgsqlDbType.Text);
                    await writer.WriteAsync(entry.Category, NpgsqlDbType.Text);
                    await writer.WriteAsync(entry.Title, NpgsqlDbType.Text);
                    await writer.WriteAsync(entry.Year, NpgsqlDbType.Text);
                    await writer.WriteAsync(entry.Adult, NpgsqlDbType.Boolean);
                }
                catch (NpgsqlException e)
                {
                    if (e.Message.Contains("duplicate key value violates unique constraint", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    
                    throw;
                }
            }

            await writer.CompleteAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while inserting imdb entries into database");
        }
    }

    public async Task EnsureIndexExistsForNewData()
    {
        try
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(configuration.StorageConnectionString)
            {
                CommandTimeout = 3000,
            };

            await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
            await connection.OpenAsync();

            await DropPgtrmIndex(connection);
            await CreatePgtrmIndex(connection);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while creating index on imdb_metadata table");
        }
    }
    
    private async Task DropPgtrmIndex(NpgsqlConnection connection)
    {
        const string checkIndexExistsCommand = "SELECT 1 FROM pg_indexes WHERE indexname = 'title_gist'";

        await using var checkCommand = new NpgsqlCommand(checkIndexExistsCommand, connection);
        var indexExists = await checkCommand.ExecuteScalarAsync();

        if (indexExists != null)
        {
            const string dropIndexCommand = "DROP INDEX title_gist";

            try
            {
                await using var dropCommand = new NpgsqlCommand(dropIndexCommand, connection);
                await dropCommand.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while dropping index on imdb_metadata table");
            }
        }
    }

    private async Task CreatePgtrmIndex(NpgsqlConnection connection)
    {
        const string sqlCommand = "CREATE INDEX title_gist ON imdb_metadata USING gist(title gist_trgm_ops)";
        
        try
        {
            await using var command = new NpgsqlCommand(sqlCommand, connection);
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while creating index on imdb_metadata table");
        }
    }
}
