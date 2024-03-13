namespace Producer.Features.PTN.Handlers;

public class ExtendedHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser) => parser.AddHandler(ResultKeys.Extended, new Regex(@"EXTENDED"), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
}