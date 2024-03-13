namespace Producer.Features.PTN.Handlers;

public class RetailHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser) => parser.AddHandler(ResultKeys.Retail, new Regex(@"\bRetail\b", RegexOptions.IgnoreCase), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
}