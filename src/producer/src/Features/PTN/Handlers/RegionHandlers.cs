namespace Producer.Features.PTN.Handlers;

public class RegionHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser) => parser.AddHandler(ResultKeys.Region, new Regex(@"R\d\b"), Transformers.None, new() { SkipIfFirst = true });
}