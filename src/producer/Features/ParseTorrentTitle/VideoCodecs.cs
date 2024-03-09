namespace Producer.Features.ParseTorrentTitle;

public sealed class VideoCodec : SmartEnum<VideoCodec, string>
{
    public static readonly VideoCodec X265 = new("X265", "x265");
    public static readonly VideoCodec X264 = new("X264", "x264");
    public static readonly VideoCodec H264 = new("H264", "h264");
    public static readonly VideoCodec H265 = new("H265", "h265");
    public static readonly VideoCodec WMV = new("WMV", "WMV");
    public static readonly VideoCodec XVID = new("XVID", "xvid");
    public static readonly VideoCodec DVDR = new("DVDR", "dvdr");

    private VideoCodec(string name, string value) : base(name, value)
    {
    }
}