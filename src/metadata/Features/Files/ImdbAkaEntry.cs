namespace Metadata.Features.Files;

public class ImdbAkaEntry
{
    public string ImdbId { get; set; } = default!;
    public int Ordering { get; set; }
    public string? LocalizedTitle { get; set; }
    public string? Region { get; set; }
    public string? Language { get; set; }
    public string? Types { get; set; }
    public string? Attributes { get; set; }
    public bool IsOriginalTitle { get; set; }
}