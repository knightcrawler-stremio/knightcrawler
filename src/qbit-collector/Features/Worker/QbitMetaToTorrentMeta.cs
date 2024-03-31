namespace QBitCollector.Features.Worker;

public static class QbitMetaToTorrentMeta
{
    public static IReadOnlyList<TorrentFile> MapMetadataToFilesCollection(
        IRankTorrentName rankTorrentName,
        Torrent torrent,
        string ImdbId,
        IReadOnlyList<TorrentContent> Metadata,
        ILogger<WriteQbitMetadataConsumer> logger)
    {
        try
        {
            var files = new List<TorrentFile>();

            foreach (var metadataEntry in Metadata.Where(m => Filetypes.VideoFileExtensions.Any(ext => m.Name.EndsWith(ext))))
            {
                var file = new TorrentFile
                {
                    ImdbId = ImdbId,
                    KitsuId = 0,
                    InfoHash = torrent.InfoHash,
                    FileIndex = metadataEntry.Index ?? 0,
                    Title = metadataEntry.Name,
                    Size = metadataEntry.Size,
                };
            
                var parsedTitle = rankTorrentName.Parse(file.Title, false);

                if (!parsedTitle.Success)
                {
                    logger.LogWarning("Failed to parse title {Title} for metadata mapping", file.Title);
                    continue;
                }
            
                file.ImdbSeason = parsedTitle.Response?.Season?.FirstOrDefault() ?? 0;
                file.ImdbEpisode = parsedTitle.Response?.Episode?.FirstOrDefault() ?? 0;
            
                files.Add(file);
            }

            return files;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Failed to map metadata to files collection: {Exception}", ex.Message);
            return [];
        }
    }
    
    public static async Task<IReadOnlyList<SubtitleFile>> MapMetadataToSubtitlesCollection(IDataStorage storage, string InfoHash, IReadOnlyList<TorrentContent> Metadata, 
        ILogger<WriteQbitMetadataConsumer> logger)
    {
        try
        {
            var files = new List<SubtitleFile>();

            var torrentFiles = await storage.GetTorrentFiles(InfoHash.ToLowerInvariant());
            
            if (torrentFiles.Count == 0)
            {
                return files;
            }

            foreach (var metadataEntry in Metadata.Where(m => Filetypes.SubtitleFileExtensions.Any(ext => m.Name.EndsWith(ext))))
            {
                var fileId = torrentFiles.FirstOrDefault(t => Path.GetFileNameWithoutExtension(t.Title) == Path.GetFileNameWithoutExtension(metadataEntry.Name))?.Id ?? 0;
            
                var file = new SubtitleFile
                {
                    InfoHash = InfoHash,
                    FileIndex = metadataEntry.Index ?? 0,
                    FileId = fileId,
                    Title = metadataEntry.Name,
                };
            
                files.Add(file);
            }

            return files;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Failed to map metadata to subtitles collection: {Exception}", ex.Message);
            return [];
        }
    }
}