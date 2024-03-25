namespace QBitCollector.Features.Worker;

public static class QbitMetaToTorrentMeta
{
    public static IReadOnlyList<TorrentFile> MapMetadataToFilesCollection(
        IParseTorrentTitle torrentTitle,
        Torrent torrent,
        string ImdbId,
        IReadOnlyList<TorrentContent> Metadata)
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
            
                var parsedTitle = torrentTitle.Parse(file.Title);
            
                file.ImdbSeason = parsedTitle.Seasons.FirstOrDefault();
                file.ImdbEpisode = parsedTitle.Episodes.FirstOrDefault();
            
                files.Add(file);
            }

            return files;
        }
        catch (Exception)
        {
            return [];
        }
    }
    
    public static async Task<IReadOnlyList<SubtitleFile>> MapMetadataToSubtitlesCollection(IDataStorage storage, string InfoHash, IReadOnlyList<TorrentContent> Metadata)
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
        catch (Exception)
        {
            return [];
        }
    }
}