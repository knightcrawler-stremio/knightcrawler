namespace Metadata.Features.ImportImdbData;

public class ImdbDbService(PostgresConfiguration configuration, ILogger<ImdbDbService> logger)
{
    public Task InsertImdbEntries(IEnumerable<ImdbBasicEntry> entries) =>
        ExecuteCommandAsync(
            async connection =>
            {
                await using var writer = await connection.BeginBinaryImportAsync(
                    $"COPY {TableNames.MetadataTable} (\"imdb_id\", \"category\", \"title\", \"year\", \"adult\") FROM STDIN (FORMAT BINARY)");

                foreach (var entry in entries)
                {
                    try
                    {
                        await writer.StartRowAsync();
                        await writer.WriteAsync(entry.ImdbId, NpgsqlDbType.Text);
                        await writer.WriteAsync(entry.Category, NpgsqlDbType.Text);
                        await writer.WriteAsync(entry.Title, NpgsqlDbType.Text);
                        await writer.WriteAsync(entry.Year, NpgsqlDbType.Integer);
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
                    $"COPY {TableNames.AkasTable} (\"imdb_id\", \"ordering\", \"localized_title\", \"region\", \"language\", \"types\", \"attributes\", \"is_original_title\") FROM STDIN (FORMAT BINARY)");

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
                    catch (PostgresException e)
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
    
    public Task InsertImdbEpisodeEntries(IEnumerable<ImdbEpisodeEntry> entries) =>
        ExecuteCommandAsync(
            async connection =>
            {
                await using var writer = await connection.BeginBinaryImportAsync(
                    $"COPY {TableNames.EpisodesTable} (\"episode_id\", \"parent_id\", \"season\", \"episode\") FROM STDIN (FORMAT BINARY)");

                foreach (var entry in entries)
                {
                    try
                    {
                        await writer.StartRowAsync();
                        await writer.WriteAsync(entry.EpisodeImdbId, NpgsqlDbType.Text);
                        await writer.WriteAsync(entry.ParentImdbId, NpgsqlDbType.Text);
                        await writer.WriteAsync(entry.SeasonNumber, NpgsqlDbType.Text);
                        await writer.WriteAsync(entry.EpisodeNumber, NpgsqlDbType.Text);
                        
                    }
                    catch (PostgresException e)
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
    
    public Task TruncateTable(string table, bool cascade = false) =>
        ExecuteCommandAsync(
            async connection =>
            {
                var cascadeOption = cascade ? "CASCADE" : string.Empty;
                logger.LogInformation("Truncating '{Table}' table", table);
                await using var command = new NpgsqlCommand($"TRUNCATE TABLE {table} {cascadeOption}", connection);
                await command.ExecuteNonQueryAsync();
            }, $"Error while clearing '{table}' table");
   
    public Task CreatePgtrmIndex() =>
        ExecuteCommandAsync(
            async connection =>
            {
                await using var command = new NpgsqlCommand($"CREATE INDEX title_gin ON {TableNames.MetadataTable} USING gin(title gin_trgm_ops)", connection);
                await command.ExecuteNonQueryAsync();
            }, "Error while creating index on imdb_metadata table");

    public Task DropPgtrmIndex() =>
        ExecuteCommandAsync(
            async connection =>
            {
                logger.LogInformation("Dropping Trigrams index if it exists already");
                await using var dropCommand = new NpgsqlCommand("DROP INDEX if exists title_gin", connection);
                await dropCommand.ExecuteNonQueryAsync();
            }, $"Error while dropping index on {TableNames.MetadataTable} table");


    private async Task ExecuteCommandAsync(Func<NpgsqlConnection, Task> operation, string errorMessage)
    {
        try
        {
            await using var connection = new NpgsqlConnection(configuration.StorageConnectionString);
            await connection.OpenAsync();

            await operation(connection);
        }
        catch (Exception e)
        {
            logger.LogError(e, errorMessage);
        }
    }

    private async Task ExecuteCommandWithTransactionAsync(Func<NpgsqlConnection, NpgsqlTransaction, Task> operation, NpgsqlTransaction transaction, string errorMessage)
    {
        try
        {
            await operation(transaction.Connection, transaction);
        }
        catch (PostgresException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception e)
        {
            logger.LogError(e, errorMessage);
        }
    }
}