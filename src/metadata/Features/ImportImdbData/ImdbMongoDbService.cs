namespace Metadata.Features.ImportImdbData;

public class ImdbMongoDbService
{
    private readonly ILogger<ImdbMongoDbService> _logger;
    private readonly IMongoCollection<ImdbEntry> _imdbCollection;

    public ImdbMongoDbService(MongoConfiguration configuration, ILogger<ImdbMongoDbService> logger)
    {
        _logger = logger;
        
        var client = new MongoClient(configuration.ConnectionString);
        var database = client.GetDatabase(configuration.DbName);

        _imdbCollection = database.GetCollection<ImdbEntry>("imdb-entries");
    }

    public async Task InsertImdbEntries(IEnumerable<ImdbEntry> entries)
    {
        var operations = new List<WriteModel<ImdbEntry>>();

        foreach (var entry in entries)
        {
            var filter = Builders<ImdbEntry>.Filter.Eq(e => e.ImdbId, entry.ImdbId);
            var update = Builders<ImdbEntry>.Update
                .SetOnInsert(e => e.TitleType, entry.TitleType)
                .SetOnInsert(e => e.PrimaryTitle, entry.PrimaryTitle)
                .SetOnInsert(e => e.OriginalTitle, entry.OriginalTitle)
                .SetOnInsert(e => e.IsAdult, entry.IsAdult)
                .SetOnInsert(e => e.StartYear, entry.StartYear)
                .SetOnInsert(e => e.EndYear, entry.EndYear)
                .SetOnInsert(e => e.RuntimeMinutes, entry.RuntimeMinutes)
                .SetOnInsert(e => e.Genres, entry.Genres);

            operations.Add(new UpdateOneModel<ImdbEntry>(filter, update) { IsUpsert = true });
        }

        await _imdbCollection.BulkWriteAsync(operations);
    }
    
    public bool IsDatabaseInitialized()
    {
        try
        {
            // Create compound index for PrimaryTitle, TitleType, and StartYear
            var indexKeysDefinition = Builders<ImdbEntry>.IndexKeys
                .Text(e => e.PrimaryTitle)
                .Ascending(e => e.TitleType)
                .Ascending(e => e.StartYear);

            var createIndexOptions = new CreateIndexOptions { Background = true };
            var indexModel = new CreateIndexModel<ImdbEntry>(indexKeysDefinition, createIndexOptions);

            _imdbCollection.Indexes.CreateOne(indexModel);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error initializing database");
            return false;
        }
    }
}