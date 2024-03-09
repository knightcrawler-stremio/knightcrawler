namespace Producer.Features.ParseTorrentTitle;

public sealed class Resolution : SmartEnum<Resolution, string>
{
    public static readonly Resolution R2160P = new("R2160P", "2160P");
    public static readonly Resolution R1080P = new("R1080P", "1080P");
    public static readonly Resolution R720P = new("R720P", "720P");
    public static readonly Resolution R576P = new("R576P", "576P");
    public static readonly Resolution R540P = new("R540P", "540P");
    public static readonly Resolution R480P = new("R480P", "480P");

    private Resolution(string name, string value) : base(name, value) { }
}
