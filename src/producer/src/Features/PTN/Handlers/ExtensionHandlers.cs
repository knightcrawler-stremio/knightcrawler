namespace Producer.Features.PTN.Handlers;

public class ExtensionHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser) => 
        parser.AddHandler(ResultKeys.Extension, new Regex(@"\.(3g2|3gp|avi|flv|mkv|mk3d|mov|mp2|mp4|m4v|mpe|mpeg|mpg|mpv|webm|wmv|ogm|divx|ts|m2ts|iso|vob|sub|idx|ttxt|txt|smi|srt|ssa|ass|vtt|nfo|html)$", RegexOptions.IgnoreCase), Transformers.Lowercase, ParseTorrentNameOptions.DefaultOptions);
}