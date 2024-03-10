namespace Producer.Features.ParseTorrentTitle;

public class ParsedFilename
{
    public ParsedMovie? Movie { get; set; }
    public ParsedTv? Show { get; set; }
    public TorrentType? Type { get; set; }

    public bool IsInvalid => Movie is null && Show is null;
}
