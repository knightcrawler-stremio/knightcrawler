namespace SharedContracts.Models;

public class TorrentFile
{
    public int Id { get; set; }
    public string InfoHash { get; set; } = default!;
    public int FileIndex { get; set; }
    public string Title { get; set; } = default!;
    public long Size { get; set; }
    public string? ImdbId { get; set; }
    public int? ImdbSeason { get; set; }
    public int? ImdbEpisode { get; set; }
    public int? KitsuId { get; set; }
    public int? KitsuEpisode { get; set; }
}