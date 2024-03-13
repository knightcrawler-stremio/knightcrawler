namespace Producer.Features.PTN.Handlers;

public class CompleteHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Complete, new Regex(@"(?:\bthe\W)?(?:\bcomplete|collection|dvd)?\b[ .]?\bbox[ .-]?set\b", RegexOptions.IgnoreCase), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Complete, new Regex(@"(?:\bthe\W)?(?:\bcomplete|collection|dvd)?\b[ .]?\bmini[ .-]?series\b", RegexOptions.IgnoreCase), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Complete, new Regex(@"(?:\bthe\W)?(?:(\bcomplete|full|all)\b).*\b(?:series|seasons|collection|episodes|set|pack|movies)\b", RegexOptions.IgnoreCase), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Complete, new Regex(@"(\b(?:series|seasons|movies?)\b.*\b(?:complete|collection)\b)", RegexOptions.IgnoreCase), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Complete, new Regex(@"(?:\bthe\W)?\bultimate\b[ .]\bcollection\b", RegexOptions.IgnoreCase), Transformers.Boolean, new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Complete, new Regex(@"(\bcollection\b.*\b(?:set|pack|movies)\b)", RegexOptions.IgnoreCase), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Complete, new Regex(@"(\bcollection\b)", RegexOptions.IgnoreCase), Transformers.Boolean, new() { SkipFromTitle = false });
        parser.AddHandler(ResultKeys.Complete, new Regex(@"(duology|trilogy|quadr[oi]logy|tetralogy|pentalogy|hexalogy|heptalogy|anthology|saga)", RegexOptions.IgnoreCase), Transformers.Boolean, new() { SkipIfAlreadyFound = false });
    }
}