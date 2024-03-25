namespace TorrentConsumer.Features.TorrentIngestion;

public class PerformIngestionConsumer(IDataStorage dataStorage, ILogger<PerformIngestionConsumer> logger, IBus messageBus) : IConsumer<PerformIngestionActions>
{
    public async Task Consume(ConsumeContext<PerformIngestionActions> context)
    {
        var request = context.Message;
        
        logger.LogInformation("Performing ingestion actions for infoHash {InfoHash} in Saga {SagaId}", request.IngestedTorrent.InfoHash, request.CorrelationId);
        
        var torrent = new Torrent
        {
            InfoHash = request.IngestedTorrent.InfoHash.ToLowerInvariant(),
            Provider = request.IngestedTorrent.Source,
            Title = request.IngestedTorrent.Name,
            Type = request.IngestedTorrent.Category,
            Seeders = request.IngestedTorrent.Seeders,
            Reviewed = false,
            Opened = false,
        };

        await dataStorage.InsertTorrent(torrent);
        
        await messageBus.Publish(new IngestionCompleted(request.CorrelationId, torrent));
    }
}