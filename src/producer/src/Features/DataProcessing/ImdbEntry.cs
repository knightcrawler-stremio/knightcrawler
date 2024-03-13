namespace Producer.Features.DataProcessing;

public class ImdbEntry
{
    public string ImdbId { get; set; } = default!;
    public string? Title { get; set; }
    public string? Category { get; set; }
    public string? Year { get; set; }
    public bool? Adult { get; set; }
}
