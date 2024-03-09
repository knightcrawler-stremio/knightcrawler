namespace Producer.Features.ParseTorrentTitle;

public static partial class AudioCodecsParser
{
    [GeneratedRegex(@"\b(?<mp3>(LAME(?:\d)+-?(?:\d)+)|(mp3))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex Mp3CodecExp();

    [GeneratedRegex(@"\b(?<mp2>(mp2))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex Mp2CodecExp();

    [GeneratedRegex(@"\b(?<dolby>(Dolby)|(Dolby-?Digital)|(DD)|(AC3D?))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex DolbyCodecExp();

    [GeneratedRegex(@"\b(?<dolbyatmos>(Dolby-?Atmos))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex DolbyAtmosCodecExp();

    [GeneratedRegex(@"\b(?<aac>(AAC))(\d?.?\d?)(ch)?\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex AacAtmosCodecExp();

    [GeneratedRegex(@"\b(?<eac3>(EAC3|DDP|DD\+))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex Eac3CodecExp();

    [GeneratedRegex(@"\b(?<flac>(FLAC))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex FlacCodecExp();

    [GeneratedRegex(@"\b(?<dts>(DTS))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex DtsCodecExp();

    [GeneratedRegex(@"\b(?<dtshd>(DTS-?HD)|(DTS(?=-?MA)|(DTS-X)))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex DtsHdCodecExp();

    [GeneratedRegex(@"\b(?<truehd>(True-?HD))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex TrueHdCodecExp();

    [GeneratedRegex(@"\b(?<opus>(Opus))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex OpusCodecExp();

    [GeneratedRegex(@"\b(?<vorbis>(Vorbis))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex VorbisCodecExp();

    [GeneratedRegex(@"\b(?<pcm>(PCM))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex PcmCodecExp();

    [GeneratedRegex(@"\b(?<lpcm>(LPCM))\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex LpcmCodecExp();

    private static readonly Regex AudioCodecExp = new(
        string.Join(
            "|", Mp3CodecExp(), Mp2CodecExp(), DolbyCodecExp(), DolbyAtmosCodecExp(), AacAtmosCodecExp(), Eac3CodecExp(), FlacCodecExp(),
            DtsHdCodecExp(),
            DtsCodecExp(), TrueHdCodecExp(), OpusCodecExp(), VorbisCodecExp(), PcmCodecExp(), LpcmCodecExp()), RegexOptions.IgnoreCase);

    public static void Parse(string title, out AudioCodec? codec, out string? source)
    {
        codec = null;
        source = null;

        var audioResult = AudioCodecExp.Match(title);

        if (!audioResult.Success)
        {
            return;
        }

        var groups = audioResult.Groups;

        if (groups["aac"].Success)
        {
            codec = AudioCodec.AAC;
            source = groups["aac"].Value;
        }
        else if (groups["dolbyatmos"].Success)
        {
            codec = AudioCodec.EAC3;
            source = groups["dolbyatmos"].Value;
        }
        else if (groups["dolby"].Success)
        {
            codec = AudioCodec.DOLBY;
            source = groups["dolby"].Value;
        }
        else if (groups["dtshd"].Success)
        {
            codec = AudioCodec.DTSHD;
            source = groups["dtshd"].Value;
        }
        else if (groups["dts"].Success)
        {
            codec = AudioCodec.DTS;
            source = groups["dts"].Value;
        }
        else if (groups["flac"].Success)
        {
            codec = AudioCodec.FLAC;
            source = groups["flac"].Value;
        }
        else if (groups["truehd"].Success)
        {
            codec = AudioCodec.TRUEHD;
            source = groups["truehd"].Value;
        }
        else if (groups["mp3"].Success)
        {
            codec = AudioCodec.MP3;
            source = groups["mp3"].Value;
        }
        else if (groups["mp2"].Success)
        {
            codec = AudioCodec.MP2;
            source = groups["mp2"].Value;
        }
        else if (groups["pcm"].Success)
        {
            codec = AudioCodec.PCM;
            source = groups["pcm"].Value;
        }
        else if (groups["lpcm"].Success)
        {
            codec = AudioCodec.LPCM;
            source = groups["lpcm"].Value;
        }
        else if (groups["opus"].Success)
        {
            codec = AudioCodec.OPUS;
            source = groups["opus"].Value;
        }
        else if (groups["vorbis"].Success)
        {
            codec = AudioCodec.VORBIS;
            source = groups["vorbis"].Value;
        }
        else if (groups["eac3"].Success)
        {
            codec = AudioCodec.EAC3;
            source = groups["eac3"].Value;
        }
    }
}