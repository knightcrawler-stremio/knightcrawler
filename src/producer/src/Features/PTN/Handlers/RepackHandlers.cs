namespace Producer.Features.PTN.Handlers;

public class RepackHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser) => parser.AddHandler(ResultKeys.Repack, new Regex(@"REPACK|RERIP"), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
}