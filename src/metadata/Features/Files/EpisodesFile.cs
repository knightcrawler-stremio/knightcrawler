namespace Metadata.Features.Files;

public class EpisodesFile(ILogger<EpisodesFile> logger, ImdbDbService dbService): IFileImport<ImdbEpisodeEntry>
{
    public async Task Import(string fileName, int batchSize, CancellationToken cancellationToken)
    {
        logger.LogInformation("Importing Downloaded IMDB Episodes data from {FilePath}", fileName);

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "\t",
            BadDataFound = null, // Skip Bad Data from Imdb
            MissingFieldFound = null, // Skip Missing Fields from Imdb
        };
        
        using var reader = new StreamReader(fileName);
        using var csv = new CsvReader(reader, csvConfig);

        var channel = Channel.CreateBounded<ImdbEpisodeEntry>(new BoundedChannelOptions(batchSize)
        {
            FullMode = BoundedChannelFullMode.Wait,
        });

        await csv.ReadAsync();

        var batchInsertTask = CreateBatchOfAkaEntries(channel, batchSize, cancellationToken);

        await ReadAkaEntries(csv, channel, cancellationToken);

        channel.Writer.Complete();

        await batchInsertTask;
    }
    
    private Task CreateBatchOfAkaEntries(Channel<ImdbEpisodeEntry, ImdbEpisodeEntry> channel, int batchSize, CancellationToken cancellationToken) =>
        Task.Run(async () =>
        {
            await foreach (var movieData in channel.Reader.ReadAllAsync(cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var batch = new List<ImdbEpisodeEntry>
                {
                    movieData,
                };

                while (batch.Count < batchSize && channel.Reader.TryRead(out var nextMovieData))
                {
                    batch.Add(nextMovieData);
                }

                if (batch.Count > 0)
                {
                    await dbService.InsertImdbEpisodeEntries(batch);
                    logger.LogInformation("Imported batch of {BatchSize} episodes starting with ImdbId {FirstImdbId}", batch.Count, batch.First().EpisodeImdbId);
                }
            }
        }, cancellationToken);
    
    private static async Task ReadAkaEntries(CsvReader csv, Channel<ImdbEpisodeEntry, ImdbEpisodeEntry> channel, CancellationToken cancellationToken)
    {
        while (await csv.ReadAsync())
        {
            var data = new ImdbEpisodeEntry
            {
                EpisodeImdbId = csv.GetField(0),
                ParentImdbId = csv.GetField(1),
                SeasonNumber = csv.GetField(2),
                EpisodeNumber = csv.GetField(3),
            };

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await channel.Writer.WriteAsync(data, cancellationToken);
        }
    }
}