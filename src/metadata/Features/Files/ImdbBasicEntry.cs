namespace Metadata.Features.Files;

public class ImdbBasicEntry
{
    public string ImdbId { get; set; } = default!;
    public string? Category { get; set; }
    public string? Title { get; set; }
    public bool Adult { get; set; }
    public int Year { get; set; }
}