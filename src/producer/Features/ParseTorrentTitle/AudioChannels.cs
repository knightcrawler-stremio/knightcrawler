namespace Producer.Features.ParseTorrentTitle;

public sealed class AudioChannels : SmartEnum<AudioChannels, string>
{
    public static readonly AudioChannels SEVEN = new("SEVEN", "7.1");
    public static readonly AudioChannels SIX = new("SIX", "5.1");
    public static readonly AudioChannels STEREO = new("STEREO", "stereo");
    public static readonly AudioChannels MONO = new ("MONO", "mono");

    private AudioChannels(string name, string value) : base(name, value)
    {
    }
}