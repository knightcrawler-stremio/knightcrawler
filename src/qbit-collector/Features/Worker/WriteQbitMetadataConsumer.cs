namespace QBitCollector.Features.Worker;

public class WriteQbitMetadataConsumer(IRankTorrentName rankTorrentName, IDataStorage dataStorage, ILogger<WriteQbitMetadataConsumer> logger) : IConsumer<WriteQbitMetadata>
{
    public async Task Consume(ConsumeContext<WriteQbitMetadata> context)
    {
        var request = context.Message;
        
        if (request.Metadata.Metadata.Count == 0)
        {
            await context.Publish(new QbitMetadataWritten(request.Metadata, false));
            return;
        }

        var torrentFiles = QbitMetaToTorrentMeta.MapMetadataToFilesCollection(
            rankTorrentName, request.Torrent, request.ImdbId, request.Metadata.Metadata, logger);

        if (!torrentFiles.Any())
        {
            await context.Publish(new QbitMetadataWritten(request.Metadata, false));
            return;
        }

        await dataStorage.InsertFiles(torrentFiles);

        var subtitles = await QbitMetaToTorrentMeta.MapMetadataToSubtitlesCollection(
            dataStorage, request.Torrent.InfoHash, request.Metadata.Metadata, logger);

        if (subtitles.Any())
        {
            await dataStorage.InsertSubtitles(subtitles);
        }

        await context.Publish(new QbitMetadataWritten(request.Metadata, true));
    }
}