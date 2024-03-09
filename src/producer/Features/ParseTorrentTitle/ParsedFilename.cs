namespace Producer.Features.ParseTorrentTitle;

public class ParsedFilename
{
    public ParsedMovie? Movie { get; set; }
    public ParsedTv? Show { get; set; }
    public bool IsMovie => Movie is not null;
    public bool IsShow => Show is not null;

    public bool IsInvalid => (!IsMovie && !IsShow) || (IsMovie && IsShow);
}