namespace Producer.Features.ParseTorrentTitle;

public static partial class AudioChannelsParser
{
    [GeneratedRegex(@"\b(?<eight>7.?[01])\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex EightChannelExp();
    [GeneratedRegex(@"\b(?<six>(6[\W]0(?:ch)?)(?=[^\d]|$)|(5[\W][01](?:ch)?)(?=[^\d]|$)|5ch|6ch)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SixChannelExp();
    [GeneratedRegex(@"(?<stereo>((2[\W]0(?:ch)?)(?=[^\d]|$))|(stereo))", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex StereoChannelExp();
    [GeneratedRegex(@"(?<mono>(1[\W]0(?:ch)?)(?=[^\d]|$)|(mono)|(1ch))", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MonoChannelExp();
    
    private static readonly Regex ChannelExp = new(string.Join("|", EightChannelExp(), SixChannelExp(), StereoChannelExp(), MonoChannelExp()), RegexOptions.IgnoreCase);

    public static void Parse(string title, out AudioChannels? channels, out string? source)
    {
        channels = null;
        source = null;

        var channelResult = ChannelExp.Match(title);
        if (!channelResult.Success)
        {
            return;
        }

        var groups = channelResult.Groups;

        if (groups["eight"].Success)
        {
            channels = AudioChannels.SEVEN;
            source = groups["eight"].Value;
        }
        else if (groups["six"].Success)
        {
            channels = AudioChannels.SIX;
            source = groups["six"].Value;
        }
        else if (groups["stereo"].Success)
        {
            channels = AudioChannels.STEREO;
            source = groups["stereo"].Value;
        }
        else if (groups["mono"].Success)
        {
            channels = AudioChannels.MONO;
            source = groups["mono"].Value;
        }
    }
}