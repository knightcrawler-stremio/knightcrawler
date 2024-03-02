namespace Producer.Features.CrawlerSupport;

// Torrent represents a crawled torrent from one of our
// supported sources.
public class Torrent
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public string? Source { get; set; }
    public string? Category { get; set; }
    public string? InfoHash { get; set; }
    public string? Size { get; set; }
    public int Seeders { get; set; }
    public int Leechers { get; set; }
    public string? Imdb { get; set; }

    public bool Processed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}