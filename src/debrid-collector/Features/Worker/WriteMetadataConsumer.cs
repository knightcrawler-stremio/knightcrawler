namespace DebridCollector.Features.Worker;

public class WriteMetadataConsumer(IRankTorrentName rankTorrentName, IDataStorage dataStorage, ILogger<WriteMetadataConsumer> logger) : IConsumer<WriteMetadata>
{
    public async Task Consume(ConsumeContext<WriteMetadata> context)
    {
        var request = context.Message;
        
        var torrentFiles = DebridMetaToTorrentMeta.MapMetadataToFilesCollection(rankTorrentName, request.Torrent, request.ImdbId, request.Metadata.Metadata, logger);

        if (!torrentFiles.Any())
        {
            await context.Publish(new MetadataWritten(request.Metadata, false));
            return;
        }
        
        await dataStorage.InsertFiles(torrentFiles);

        var subtitles = await DebridMetaToTorrentMeta.MapMetadataToSubtitlesCollection(dataStorage, request.Torrent.InfoHash, request.Metadata.Metadata, logger);

        if (subtitles.Any())
        {
            await dataStorage.InsertSubtitles(subtitles);
        }
        
        await context.Publish(new MetadataWritten(request.Metadata, true));
    }
}