namespace Producer.Features.ParseTorrentTitle;

public class QualityModel
{
    public List<Source> Sources { get; set; } = [];
    public QualityModifier? Modifier { get; set; }
    public Resolution? Resolution { get; set; }
    public Revision Revision { get; set; } = new();
}