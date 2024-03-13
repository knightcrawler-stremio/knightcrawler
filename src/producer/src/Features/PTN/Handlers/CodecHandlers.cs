namespace Producer.Features.PTN.Handlers;

public class CodecHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Codec, new Regex(@"\bhevc(?:\s?10)?\b", RegexOptions.IgnoreCase), Transformers.Value("hevc"), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Codec, new Regex(@"\b((?:dvix|mpeg2|divx|xvid|avc))\b", RegexOptions.IgnoreCase), Transformers.Lowercase, new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Codec, new Regex(@"\b([xh][-.\s]?26[45])\b", RegexOptions.IgnoreCase), Transformers.Clean, new() { Remove = true });
    }
}