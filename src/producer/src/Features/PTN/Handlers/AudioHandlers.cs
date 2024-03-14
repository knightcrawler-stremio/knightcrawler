namespace Producer.Features.PTN.Handlers;

public class AudioHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Audio, new Regex(@"7\.1[. ]?Atmos\b", RegexOptions.IgnoreCase), Transformers.Value("7.1 Atmos"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Audio, new Regex(@"\bEAC-?3(?:[. -]?[256]\.[01])?", RegexOptions.IgnoreCase), Transformers.Value("eac3"), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Audio, new Regex(@"\bAC-?3(?:[.-]5\.1|x2\.?0?)?\b", RegexOptions.IgnoreCase), Transformers.Value("ac3"), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Audio, new Regex(@"\b5\.1ch\b", RegexOptions.IgnoreCase), Transformers.Value("ac3"), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Audio, new Regex(@"\bDD5[. ]?1\b", RegexOptions.IgnoreCase), Transformers.Value("dd5.1"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Audio, new Regex(@"\bQ?AAC(?:[. ]?2[. ]0|x2)?\b", RegexOptions.IgnoreCase), Transformers.Value("aac"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Audio, new Regex(@"\bFLAC(?:\+?2\.0)?(?:x[2-4])?\b", RegexOptions.IgnoreCase), Transformers.Value("flac"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Audio, new Regex(@"\b(mp3|Atmos|DTS(?:-HD)?|TrueHD)\b", RegexOptions.IgnoreCase), Transformers.Lowercase, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Audio, new Regex(@"\bmd\b", RegexOptions.IgnoreCase), Transformers.Value("md"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Audio, new Regex(@"\b5\.1(?:x[2-4]+)?\+2\.0(?:x[2-4])?\b", RegexOptions.IgnoreCase), Transformers.Value("2.0"), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Audio, new Regex(@"\b2\.0(?:x[2-4]|\+5\.1(?:x[2-4])?)\b", RegexOptions.IgnoreCase), Transformers.Value("2.0"), new() { Remove = true, SkipIfAlreadyFound = false });
    }
}