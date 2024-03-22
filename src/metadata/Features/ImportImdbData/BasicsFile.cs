namespace Metadata.Features.ImportImdbData;

public static class BasicsFile
{
    public static async Task Import(string fileName, ILogger<ImportImdbDataRequestHandler> logger, ImdbDbService dbService, int batchSize, CancellationToken cancellationToken)
    {
        logger.LogInformation("Importing Downloaded IMDB Basics data from {FilePath}", fileName);

        logger.LogInformation("Truncating 'imdb_metadata' table");
        
        await dbService.DropPgtrmIndex();
        
        await dbService.TruncateTable("imdb_metadata");
        
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "\t",
            BadDataFound = null, // Skip Bad Data from Imdb
            MissingFieldFound = null, // Skip Missing Fields from Imdb
        };
        
        using var reader = new StreamReader(fileName);
        using var csv = new CsvReader(reader, csvConfig);

        var channel = Channel.CreateBounded<ImdbEntry>(new BoundedChannelOptions(batchSize)
        {
            FullMode = BoundedChannelFullMode.Wait,
        });

        await csv.ReadAsync();

        var batchInsertTask = CreateBatchOfBasicEntries(channel, logger, dbService, batchSize, cancellationToken);

        await ReadBasicEntries(csv, channel, cancellationToken);

        channel.Writer.Complete();

        await batchInsertTask;
    }
    
    private static Task CreateBatchOfBasicEntries(Channel<ImdbEntry, ImdbEntry> channel, ILogger<ImportImdbDataRequestHandler> logger, ImdbDbService dbService, int batchSize, CancellationToken cancellationToken) =>
        Task.Run(async () =>
        {
            await foreach (var movieData in channel.Reader.ReadAllAsync(cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var batch = new List<ImdbEntry>
                {
                    movieData,
                };

                while (batch.Count < batchSize && channel.Reader.TryRead(out var nextMovieData))
                {
                    batch.Add(nextMovieData);
                }

                if (batch.Count > 0)
                {
                    await dbService.InsertImdbEntries(batch);
                    logger.LogInformation("Imported batch of {BatchSize} starting with ImdbId {FirstImdbId}", batch.Count, batch.First().ImdbId);
                }
            }
        }, cancellationToken);
    
    private static async Task ReadBasicEntries(CsvReader csv, Channel<ImdbEntry, ImdbEntry> channel, CancellationToken cancellationToken)
    {
        while (await csv.ReadAsync())
        {
            var isAdultSet = int.TryParse(csv.GetField(4), out var adult);
            
            var movieData = new ImdbEntry
            {
                ImdbId = csv.GetField(0),
                Category = csv.GetField(1),
                Title = csv.GetField(2),
                Adult = isAdultSet && adult == 1,
                Year = csv.GetField(5),
            };

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await channel.Writer.WriteAsync(movieData, cancellationToken);
        }
    }
}