namespace Producer.Features.ParseTorrentTitle;

public partial class VideoCodecsParser
{
    [GeneratedRegex(@"(?<x265>x265)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex X265Exp();

    [GeneratedRegex(@"(?<h265>h265)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex H265Exp();

    [GeneratedRegex(@"(?<x264>x264)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex X264Exp();

    [GeneratedRegex(@"(?<h264>h264)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex H264Exp();

    [GeneratedRegex(@"(?<wmv>WMV)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex WMVExp();

    [GeneratedRegex(@"(?<xvidhd>XvidHD)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex XvidhdExp();

    [GeneratedRegex(@"(?<xvid>X-?vid)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex XvidExp();

    [GeneratedRegex(@"(?<divx>divx)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex DivxExp();

    [GeneratedRegex(@"(?<hevc>HEVC)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex HevcExp();

    [GeneratedRegex(@"(?<dvdr>DVDR)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex DvdrExp();

    private static readonly Regex CodecExp = new(
        string.Join(
            "|", X265Exp(), H265Exp(), X264Exp(), H264Exp(), WMVExp(), XvidhdExp(), XvidExp(), DivxExp(), HevcExp(), DvdrExp()), RegexOptions.IgnoreCase);
    
    public static void Parse(string title, out VideoCodec? codec, out string? source)
    {
        codec = null;
        source = null;

        var result = CodecExp.Match(title);
        
        if (!result.Success)
        {
            return;
        }

        var groups = result.Groups;

        if (groups["h264"].Success)
        {
            codec = VideoCodec.H264;
            source = groups["h264"].Value;
        }
        else if (groups["h265"].Success)
        {
            codec = VideoCodec.H265;
            source = groups["h265"].Value;
        }
        else if (groups["x265"].Success || groups["hevc"].Success)
        {
            codec = VideoCodec.X265;
            source = groups["x265"].Success ? groups["x265"].Value : groups["hevc"].Value;
        }
        else if (groups["x264"].Success)
        {
            codec = VideoCodec.X264;
            source = groups["x264"].Value;
        }
        else if (groups["xvidhd"].Success || groups["xvid"].Success || groups["divx"].Success)
        {
            codec = VideoCodec.XVID;
            source = groups["xvidhd"].Success ? groups["xvidhd"].Value : (groups["xvid"].Success ? groups["xvid"].Value : groups["divx"].Value);
        }
        else if (groups["wmv"].Success)
        {
            codec = VideoCodec.WMV;
            source = groups["wmv"].Value;
        }
        else if (groups["dvdr"].Success)
        {
            codec = VideoCodec.DVDR;
            source = groups["dvdr"].Value;
        }
    }
}