namespace SharedContracts.Models;

public class Torrent
{
    public string? InfoHash { get; set; }
    public long? IngestedTorrentId { get; set; }
    public string? Provider { get; set; }
    public string? TorrentId { get; set; }
    public string? Title { get; set; }
    public long? Size { get; set; }
    public string? Type { get; set; }
    public string? UploadDate { get; set; }
    public int? Seeders { get; set; }
    public string? Trackers { get; set; }
    public string? Languages { get; set; }
    public string? Resolution { get; set; }
    public bool? Reviewed { get; set; }
    public bool? Opened { get; set; }
    public string? CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
}