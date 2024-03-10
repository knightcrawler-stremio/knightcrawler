namespace Producer.Features.ParseTorrentTitle;

public class BaseParsed
{
    public string? ReleaseTitle { get; set; }
    public string? Title { get; set; }
    public string? Year { get; set; }
    public Edition? Edition { get; set; }
    public Resolution? Resolution { get; set; }
    public VideoCodec? VideoCodec { get; set; }
    public AudioCodec? AudioCodec { get; set; }
    public AudioChannels? AudioChannels { get; set; }
    public Revision? Revision { get; set; }
    public string? Group { get; set; }
    public List<Language> Languages { get; set; } = [];
    public List<Source> Sources { get; set; } = [];
    public bool? Multi { get; set; }
    public bool? Complete { get; set; }
}
