namespace Metadata.Features.ImportImdbData;

public static class AkasFile
{
    public static async Task Import(string fileName, ILogger<ImportImdbDataRequestHandler> logger, ImdbDbService dbService, int batchSize, CancellationToken cancellationToken)
    {
        logger.LogInformation("Importing Downloaded IMDB AKAs data from {FilePath}", fileName);
        
        logger.LogInformation("Truncating 'imdb_metadata_akas' table");
        
        await dbService.TruncateTable("imdb_metadata_akas");

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "\t",
            BadDataFound = null, // Skip Bad Data from Imdb
            MissingFieldFound = null, // Skip Missing Fields from Imdb
        };
        
        using var reader = new StreamReader(fileName);
        using var csv = new CsvReader(reader, csvConfig);

        var channel = Channel.CreateBounded<ImdbAkaEntry>(new BoundedChannelOptions(batchSize)
        {
            FullMode = BoundedChannelFullMode.Wait,
        });

        await csv.ReadAsync();

        var batchInsertTask = CreateBatchOfAkaEntries(channel, logger, dbService, batchSize, cancellationToken);

        await ReadAkaEntries(csv, channel, cancellationToken);

        channel.Writer.Complete();

        await batchInsertTask;
    }
    
    private static Task CreateBatchOfAkaEntries(Channel<ImdbAkaEntry, ImdbAkaEntry> channel, ILogger<ImportImdbDataRequestHandler> logger, ImdbDbService dbService, int batchSize, CancellationToken cancellationToken) =>
        Task.Run(async () =>
        {
            await foreach (var movieData in channel.Reader.ReadAllAsync(cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var batch = new List<ImdbAkaEntry>
                {
                    movieData,
                };

                while (batch.Count < batchSize && channel.Reader.TryRead(out var nextMovieData))
                {
                    batch.Add(nextMovieData);
                }

                if (batch.Count > 0)
                {
                    await dbService.InsertImdbAkaEntries(batch);
                    logger.LogInformation("Imported batch of {BatchSize} starting with ImdbId {FirstImdbId}", batch.Count, batch.First().ImdbId);
                }
            }
        }, cancellationToken);
    
    private static async Task ReadAkaEntries(CsvReader csv, Channel<ImdbAkaEntry, ImdbAkaEntry> channel, CancellationToken cancellationToken)
    {
        while (await csv.ReadAsync())
        {
            var data = new ImdbAkaEntry
            {
                ImdbId = csv.GetField(0),
                Ordering = csv.GetField<int>(1),
                LocalizedTitle = csv.GetField(2),
                Region = csv.GetField(3),
                Language = csv.GetField(4),
                Types = csv.GetField(5),
                Attributes = csv.GetField(6),
            };
            
            var isOriginalTitle = int.TryParse(csv.GetField(7), out var original);
            data.IsOriginalTitle = isOriginalTitle && original == 1;

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await channel.Writer.WriteAsync(data, cancellationToken);
        }
    }
}