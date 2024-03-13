namespace Producer.Features.PTN.Handlers;

public class ConvertHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser) => parser.AddHandler(ResultKeys.Convert, new Regex(@"(CONVERT)"), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
}