namespace Producer.Features.DataProcessing;

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
    
    public async Task<IReadOnlyList<ImdbEntry>> GetImdbEntriesForRequests(string startYear, int requestLimit, string? startingId = null)
    {
        var sort = Builders<ImdbEntry>.Sort
            .Descending(e => e.StartYear)
            .Descending(e => e.ImdbId);
        
        var filter = Builders<ImdbEntry>.Filter
            .And(
                Builders<ImdbEntry>.Filter.Eq(e => e.TitleType, "movie"),
                Builders<ImdbEntry>.Filter.Lte(e => e.StartYear, startYear)
            );
        
        if (!string.IsNullOrWhiteSpace(startingId))
        {
            filter = Builders<ImdbEntry>.Filter.And(filter, Builders<ImdbEntry>.Filter.Lt(e => e.ImdbId, startingId));
        }

        return await _imdbCollection.Find(filter).Limit(requestLimit).Sort(sort).ToListAsync();
    }
    
    public async Task<long> GetTotalCountAsync()
    {
        var filter = Builders<ImdbEntry>.Filter.Eq(x => x.TitleType, "movie");
        return await _imdbCollection.CountDocumentsAsync(filter);
    }

    public bool IsDatabaseInitialized()
    {
        try
        {
            // Compound index for PrimaryTitle, TitleType, and StartYear
            var index1KeysDefinition = Builders<ImdbEntry>.IndexKeys
                .Text(e => e.PrimaryTitle)
                .Ascending(e => e.TitleType)
                .Ascending(e => e.StartYear);
            
            CreateIndex(index1KeysDefinition);

            // Compound index for StartYear and _id in descending order
            var index2KeysDefinition = Builders<ImdbEntry>.IndexKeys
                .Descending(e => e.StartYear)
                .Descending(e => e.ImdbId);
            
            CreateIndex(index2KeysDefinition);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error initializing database");
            return false;
        }
    }
    
    private void CreateIndex(IndexKeysDefinition<ImdbEntry> keysDefinition)
    {
        var createIndexOptions = new CreateIndexOptions { Background = true };
        var indexModel = new CreateIndexModel<ImdbEntry>(keysDefinition, createIndexOptions);
        _imdbCollection.Indexes.CreateOne(indexModel);
    }
}