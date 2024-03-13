namespace Producer.Features.PTN.Handlers;

public class HardcodedHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser) => parser.AddHandler(ResultKeys.Hardcoded, new Regex(@"HC|HARDCODED"), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
}