namespace Tissue.Features.DataProcessing;

public class Torrent
{
    public string? InfoHash { get; set; }
    public string? Provider { get; set; }
    public string? TorrentId { get; set; }
    public string? Title { get; set; }
    public long? Size { get; set; }
    public string? Type { get; set; }
    public DateTime UploadDate { get; set; }
    public short? Seeders { get; set; }
    public string? Trackers { get; set; }
    public string? Languages { get; set; }
    public string? Resolution { get; set; }
    public bool Reviewed { get; set; }
    public bool Opened { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
