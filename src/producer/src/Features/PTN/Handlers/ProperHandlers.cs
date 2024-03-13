namespace Producer.Features.PTN.Handlers;

public class ProperHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser) => parser.AddHandler(ResultKeys.Proper, new Regex(@"(?:REAL.)?PROPER"), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
}