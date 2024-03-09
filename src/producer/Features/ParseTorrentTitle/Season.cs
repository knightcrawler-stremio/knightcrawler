namespace Producer.Features.ParseTorrentTitle;

public class Season
{
    public string? ReleaseTitle { get; set; }
    public string? SeriesTitle { get; set; }
    public List<int> Seasons { get; set; } = [];
    public List<int> EpisodeNumbers { get; set; } = [];
    public DateTime? AirDate { get; set; }
    public bool FullSeason { get; set; }
    public bool IsPartialSeason { get; set; }
    public bool IsMultiSeason { get; set; }
    public bool IsSeasonExtra { get; set; }
    public bool IsSpecial { get; set; }
    public int SeasonPart { get; set; }
}