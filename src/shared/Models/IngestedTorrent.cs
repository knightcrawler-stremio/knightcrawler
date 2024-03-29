namespace SharedContracts.Models;

public class IngestedTorrent
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

    public bool Processed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public string? RtnResponse { get; set; }
}
