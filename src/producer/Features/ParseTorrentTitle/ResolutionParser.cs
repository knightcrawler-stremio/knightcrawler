namespace Producer.Features.ParseTorrentTitle;

public partial class ResolutionParser
{
    [GeneratedRegex(@"(?<R2160P>2160p|4k[-_. ](?:UHD|HEVC|BD)|(?:UHD|HEVC|BD)[-_. ]4k|\b(4k)\b|COMPLETE.UHD|UHD.COMPLETE)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex R2160pExp();

    [GeneratedRegex(@"(?<R1080P>1080(i|p)|1920x1080)(10bit)?", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex R1080pExp();

    [GeneratedRegex(@"(?<R720P>720(i|p)|1280x720|960p)(10bit)?", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex R720pExp();

    [GeneratedRegex(@"(?<R576P>576(i|p))", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex R576pExp();

    [GeneratedRegex(@"(?<R540P>540(i|p))", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex R540pExp();

    [GeneratedRegex(@"(?<R480P>480(i|p)|640x480|848x480)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex R480Exp();

    private static readonly Regex ResolutionExp = new(string.Join("|", R2160pExp(), R1080pExp(), R720pExp(), R576pExp(), R540pExp(), R480Exp()), RegexOptions.IgnoreCase);
    
    public static void Parse(string title, out Resolution? resolution, out string? source)
    {
        resolution = null;
        source = null;

        var result = ResolutionExp.Match(title);

        if (result.Success)
        {
            foreach (var key in Enum.GetNames(typeof(Resolution)))
            {
                if (result.Groups[key].Success)
                {
                    resolution = Resolution.FromName(key);
                    source = result.Groups[key].Value;
                    return;
                }
            }
        }

        // Fallback to guessing from some sources
        // Make safe assumptions like dvdrip is probably 480p
        SourceParser.Parse(title, out var sourceList);
        if (sourceList.Contains(Source.DVD))
        {
            resolution = Resolution.R480P;
        }
    }
}