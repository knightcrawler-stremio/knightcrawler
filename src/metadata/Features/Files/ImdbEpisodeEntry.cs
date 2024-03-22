namespace Metadata.Features.Files;

public class ImdbEpisodeEntry
{
    public string EpisodeImdbId { get; set; } = default!;
    public string? ParentImdbId { get; set; }
    public string? SeasonNumber { get; set; }
    public string? EpisodeNumber { get; set; }
}