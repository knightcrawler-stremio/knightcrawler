namespace Producer.Features.PTN;

public class ParsedTorrentResult
{
    public string? Title { get; set; }
    public string? Date { get; set; }
    public string? Year { get; set; }
    public string? Resolution { get; set; }
    public bool Extended { get; set; }
    public bool Unrated { get; set; }
    public bool Proper { get; set; }
    public bool Repack { get; set; }
    public bool Convert { get; set; }
    public bool Hardcoded { get; set; }
    public bool Retail { get; set; }
    public bool Remastered { get; set; }
    public bool Complete { get; set; }
    public string? Region { get; set; }
    public string? Container { get; set; }
    public string? Extension { get; set; }
    public string? Source { get; set; }
    public string? Codec { get; set; }
    public string? BitDepth { get; set; }
    public List<string> Hdr { get; set; } = [];
    public string? Audio { get; set; }
    public string? Group { get; set; }
    public List<int> Volumes { get; set; } = [];
    public List<int> Seasons { get; set; } = [];
    public List<int> Episodes { get; set; } = [];
    public string? Languages { get; set; }
    public bool Dubbed { get; set; }
}