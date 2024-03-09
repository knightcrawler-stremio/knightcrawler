namespace Producer.Features.ParseTorrentTitle;

public sealed class QualityModifier : SmartEnum<QualityModifier, string>
{
    public static readonly QualityModifier REMUX = new("REMUX", "REMUX");
    public static readonly QualityModifier BRDISK = new("BRDISK", "BRDISK");
    public static readonly QualityModifier RAWHD = new("RAWHD", "RAWHD");

    private QualityModifier(string name, string value) : base(name, value) { }
}