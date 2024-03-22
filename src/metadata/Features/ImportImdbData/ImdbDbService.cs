namespace Metadata.Features.ImportImdbData;

public class ImdbDbService(PostgresConfiguration configuration, ILogger<ImdbDbService> logger)
{
    public Task InsertImdbEntries(IEnumerable<ImdbEntry> entries) =>
        ExecuteCommandAsync(
            async connection =>
            {
                await using var writer = await connection.BeginBinaryImportAsync(
                    "COPY imdb_metadata (\"imdb_id\", \"category\", \"title\", \"year\", \"adult\") FROM STDIN (FORMAT BINARY)");

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
                    catch (Npgsql.PostgresException e)
                    {
                        if (e.Message.Contains("duplicate key value violates unique constraint", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        throw;
                    }
                }

                await writer.CompleteAsync();
            }, "Error while inserting imdb entries into database");
    
    public Task InsertImdbAkaEntries(IEnumerable<ImdbAkaEntry> entries) =>
        ExecuteCommandAsync(
            async connection =>
            {
                await using var writer = await connection.BeginBinaryImportAsync(
                    "COPY imdb_metadata_akas (\"imdb_id\", \"ordering\", \"localized_title\", \"region\", \"language\", \"types\", \"attributes\", \"is_original_title\") FROM STDIN (FORMAT BINARY)");

                foreach (var entry in entries.Where(x=>x.LocalizedTitle?.Length <= 8000))
                {
                    try
                    {
                        await writer.StartRowAsync();
                        await writer.WriteAsync(entry.ImdbId, NpgsqlDbType.Text);
                        await writer.WriteAsync(entry.Ordering, NpgsqlDbType.Integer);
                        await writer.WriteAsync(entry.LocalizedTitle, NpgsqlDbType.Text);
                        await writer.WriteAsync(entry.Region, NpgsqlDbType.Text);
                        await writer.WriteAsync(entry.Language, NpgsqlDbType.Text);
                        await writer.WriteAsync(entry.Types, NpgsqlDbType.Text);
                        await writer.WriteAsync(entry.Attributes, NpgsqlDbType.Text);
                        await writer.WriteAsync(entry.IsOriginalTitle, NpgsqlDbType.Boolean);
                        
                    }
                    catch (Npgsql.PostgresException e)
                    {
                        if (e.Message.Contains("value too long for type character", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        throw;
                    }
                }

                await writer.CompleteAsync();
            }, "Error while inserting imdb entries into database");
    
    public Task TruncateTable(string table) =>
        ExecuteCommandAsync(
            async connection =>
            {
                await using var command = new NpgsqlCommand($"TRUNCATE TABLE {table}", connection);
                await command.ExecuteNonQueryAsync();
            }, "Error while clearing 'imdb_metadata' table");

    public Task CreatePgtrmIndex() =>
        ExecuteCommandAsync(
            async connection =>
            {
                await using var command = new NpgsqlCommand("CREATE INDEX title_gist ON imdb_metadata USING gist(title gist_trgm_ops)", connection);
                await command.ExecuteNonQueryAsync();
            }, "Error while creating index on imdb_metadata table");

    public Task DropPgtrmIndex() =>
        ExecuteCommandAsync(
            async connection =>
            {
                await using var dropCommand = new NpgsqlCommand("DROP INDEX if exists title_gist", connection);
                await dropCommand.ExecuteNonQueryAsync();
            }, "Error while dropping index on imdb_metadata table");


    private async Task ExecuteCommandAsync(Func<NpgsqlConnection, Task> operation, string errorMessage)
    {
        try
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(configuration.StorageConnectionString)
            {
                CommandTimeout = 3000,
            };

            await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
            await connection.OpenAsync();

            await operation(connection);
        }
        catch (Exception e)
        {
            logger.LogError(e, errorMessage);
        }
    }
}