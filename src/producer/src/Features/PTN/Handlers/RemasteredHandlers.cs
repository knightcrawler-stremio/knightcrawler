namespace Producer.Features.PTN.Handlers;

public class RemasteredHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser) => parser.AddHandler(ResultKeys.Remastered, new Regex(@"\bRemaster(?:ed)?\b", RegexOptions.IgnoreCase), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
}