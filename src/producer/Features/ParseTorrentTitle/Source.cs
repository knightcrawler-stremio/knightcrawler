namespace Producer.Features.ParseTorrentTitle;

public sealed class Source : SmartEnum<Source, string>
{
    public static readonly Source BLURAY = new("BLURAY", "BLURAY");
    public static readonly Source WEBDL = new("WEBDL", "WEBDL");
    public static readonly Source WEBRIP = new("WEBRIP", "WEBRIP");
    public static readonly Source DVD = new("DVD", "DVD");
    public static readonly Source CAM = new("CAM", "CAM");
    public static readonly Source SCREENER = new("SCREENER", "SCREENER");
    public static readonly Source PPV = new("PPV", "PPV");
    public static readonly Source TELESYNC = new("TELESYNC", "TELESYNC");
    public static readonly Source TELECINE = new("TELECINE", "TELECINE");
    public static readonly Source WORKPRINT = new("WORKPRINT", "WORKPRINT");
    public static readonly Source TV = new("TV", "TV");

    private Source(string name, string value) : base(name, value)
    {
    }
}