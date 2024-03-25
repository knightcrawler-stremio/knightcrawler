namespace DebridCollector.Features.Worker;

public class WriteMetadataConsumer(IParseTorrentTitle parseTorrentTitle, IDataStorage dataStorage) : IConsumer<WriteMetadata>
{
    public async Task Consume(ConsumeContext<WriteMetadata> context)
    {
        var request = context.Message;
        
        var torrentFiles = DebridMetaToTorrentMeta.MapMetadataToFilesCollection(parseTorrentTitle, request.Torrent, request.ImdbId, request.Metadata.Metadata);

        if (torrentFiles.Any())
        {
            await dataStorage.InsertFiles(torrentFiles);

            var subtitles = await DebridMetaToTorrentMeta.MapMetadataToSubtitlesCollection(dataStorage, request.Torrent.InfoHash, request.Metadata.Metadata);

            if (subtitles.Any())
            {
                await dataStorage.InsertSubtitles(subtitles);
            }
        }
        
        await context.Publish(new MetadataWritten(request.Metadata));
    }
}