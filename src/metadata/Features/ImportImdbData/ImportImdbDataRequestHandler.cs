namespace Metadata.Features.ImportImdbData;

public class ImportImdbDataRequestHandler(ILogger<ImportImdbDataRequestHandler> logger, ImdbMongoDbService mongoDbService, JobConfiguration configuration)
{
    public async Task<DeleteDownloadedImdbDataRequest> Handle(ImportImdbDataRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Importing Downloaded IMDB data from {FilePath}", request.FilePath);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "\t",
            BadDataFound = null, // Skip Bad Data from Imdb
            MissingFieldFound = null, // Skip Missing Fields from Imdb
        };

        using var reader = new StreamReader(request.FilePath);
        using var csv = new CsvReader(reader, config);

        var channel = Channel.CreateBounded<ImdbEntry>(new BoundedChannelOptions(configuration.InsertBatchSize)
        {
            FullMode = BoundedChannelFullMode.Wait,
        });


        // Skip the header row
        await csv.ReadAsync();

        var batchInsertTask = CreateBatchOfEntries(channel, cancellationToken);

        await ReadEntries(csv, channel, cancellationToken);

        channel.Writer.Complete();

        await batchInsertTask;

        return new(request.FilePath);
    }

    private Task CreateBatchOfEntries(Channel<ImdbEntry, ImdbEntry> channel, CancellationToken cancellationToken) =>
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

                while (batch.Count < configuration.InsertBatchSize && channel.Reader.TryRead(out var nextMovieData))
                {
                    batch.Add(nextMovieData);
                }

                if (batch.Count > 0)
                {
                    await mongoDbService.InsertImdbEntries(batch);
                    logger.LogInformation("Imported batch of {BatchSize} starting with ImdbId {FirstImdbId}", batch.Count, batch.First().ImdbId);
                }
            }
        }, cancellationToken);

    private static async Task ReadEntries(CsvReader csv, Channel<ImdbEntry, ImdbEntry> channel, CancellationToken cancellationToken)
    {
        while (await csv.ReadAsync())
        {
            var movieData = new ImdbEntry
            {
                ImdbId = csv.GetField(0),
                TitleType = csv.GetField(1),
                PrimaryTitle = csv.GetField(2),
                OriginalTitle = csv.GetField(3),
                IsAdult = csv.GetField(4),
                StartYear = csv.GetField(5),
                EndYear = csv.GetField(6),
                RuntimeMinutes = csv.GetField(7),
                Genres = csv.GetField(8),
            };

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await channel.Writer.WriteAsync(movieData, cancellationToken);
        }
    }
}
