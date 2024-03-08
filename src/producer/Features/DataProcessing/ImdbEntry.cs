namespace Producer.Features.DataProcessing;

public class ImdbEntry
{
    [BsonId]
    public string ImdbId { get; set; } = default!;
    public string? TitleType { get; set; }
    public string? PrimaryTitle { get; set; }
    public string? OriginalTitle { get; set; }
    public string? IsAdult { get; set; }
    public string? StartYear { get; set; }
    public string? EndYear { get; set; }
    public string? RuntimeMinutes { get; set; }
    public string? Genres { get; set; }
}
