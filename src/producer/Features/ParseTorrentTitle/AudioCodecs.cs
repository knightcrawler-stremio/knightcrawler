namespace Producer.Features.ParseTorrentTitle;

public sealed class AudioCodec : SmartEnum<AudioCodec, string>
{
    public static readonly AudioCodec MP3 = new("MP3", "MP3");
    public static readonly AudioCodec MP2 = new("MP2", "MP2");
    public static readonly AudioCodec DOLBY = new("DOLBY", "Dolby Digital");
    public static readonly AudioCodec EAC3 = new("EAC3", "Dolby Digital Plus");
    public static readonly AudioCodec AAC = new("AAC", "AAC");
    public static readonly AudioCodec FLAC = new("FLAC", "FLAC");
    public static readonly AudioCodec DTS = new("DTS", "DTS");
    public static readonly AudioCodec DTSHD = new("DTSHD", "DTS-HD");
    public static readonly AudioCodec TRUEHD = new("TRUEHD", "Dolby TrueHD");
    public static readonly AudioCodec OPUS = new("OPUS", "Opus");
    public static readonly AudioCodec VORBIS = new("VORBIS", "Vorbis");
    public static readonly AudioCodec PCM = new("PCM", "PCM");
    public static readonly AudioCodec LPCM = new("LPCM", "LPCM");

    private AudioCodec(string name, string value) : base(name, value)
    {
    }
}