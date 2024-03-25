namespace DebridCollector.Features.Worker;

public static class DebridMetaToTorrentMeta
{
    public static IReadOnlyList<TorrentFile> MapMetadataToFilesCollection(
        IParseTorrentTitle torrentTitle,
        Torrent torrent,
        string ImdbId,
        FileDataDictionary Metadata)
    {
        try
        {
            var files = new List<TorrentFile>();

            foreach (var metadataEntry in Metadata.Where(m => Filetypes.VideoFileExtensions.Any(ext => m.Value.Filename.EndsWith(ext))))
            {
                var validFileIndex = int.TryParse(metadataEntry.Key, out var fileIndex);

                var file = new TorrentFile
                {
                    ImdbId = ImdbId,
                    KitsuId = 0,
                    InfoHash = torrent.InfoHash,
                    FileIndex = validFileIndex ? fileIndex : 0,
                    Title = metadataEntry.Value.Filename,
                    Size = metadataEntry.Value.Filesize.GetValueOrDefault(),
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
    
    public static async Task<IReadOnlyList<SubtitleFile>> MapMetadataToSubtitlesCollection(IDataStorage storage, string InfoHash, FileDataDictionary Metadata)
    {
        try
        {
            var files = new List<SubtitleFile>();

            var torrentFiles = await storage.GetTorrentFiles(InfoHash.ToLowerInvariant());
            
            if (torrentFiles.Count == 0)
            {
                return files;
            }

            foreach (var metadataEntry in Metadata.Where(m => Filetypes.SubtitleFileExtensions.Any(ext => m.Value.Filename.EndsWith(ext))))
            {
                var validFileIndex = int.TryParse(metadataEntry.Key, out var fileIndex);
                var fileId = torrentFiles.FirstOrDefault(
                    t => Path.GetFileNameWithoutExtension(t.Title) == Path.GetFileNameWithoutExtension(metadataEntry.Value.Filename))?.Id ?? 0;

                var file = new SubtitleFile
                {
                    InfoHash = InfoHash,
                    FileIndex = validFileIndex ? fileIndex : 0,
                    FileId = fileId,
                    Title = metadataEntry.Value.Filename,
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