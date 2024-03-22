namespace Metadata.Features.ImportImdbData;

public class ImdbEntry
{
    public string ImdbId { get; set; } = default!;
    public string? Category { get; set; }
    public string? Title { get; set; }
    public bool Adult { get; set; }
    public string? Year { get; set; }
}