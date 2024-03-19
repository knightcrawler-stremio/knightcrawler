namespace Metadata.Features.ImportImdbData;

public class ImdbRedisDbService(IConnectionMultiplexer redisConnection)
{
    private readonly IDatabase database = redisConnection.GetDatabase();

    public async Task InsertImdbEntries(IEnumerable<ImdbEntry> entries)
    {
        var json = database.JSON();
        
        foreach (var entry in entries)
        {
            await json.SetAsync(entry.RedisKey, "$", entry);
        }
    }
}
