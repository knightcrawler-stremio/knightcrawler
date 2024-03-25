namespace QBitCollector.Features.Worker;

public class WriteQbitMetadataConsumer(IParseTorrentTitle parseTorrentTitle, IDataStorage dataStorage) : IConsumer<WriteQbitMetadata>
{
    public async Task Consume(ConsumeContext<WriteQbitMetadata> context)
    {
        var request = context.Message;
        
        var torrentFiles = QbitMetaToTorrentMeta.MapMetadataToFilesCollection(parseTorrentTitle, request.Torrent, request.ImdbId, request.Metadata.Metadata);

        if (torrentFiles.Any())
        {
            await dataStorage.InsertFiles(torrentFiles);

            var subtitles = await QbitMetaToTorrentMeta.MapMetadataToSubtitlesCollection(dataStorage, request.Torrent.InfoHash, request.Metadata.Metadata);

            if (subtitles.Any())
            {
                await dataStorage.InsertSubtitles(subtitles);
            }
        }
        
        await context.Publish(new QbitMetadataWritten(request.Metadata));
    }
}