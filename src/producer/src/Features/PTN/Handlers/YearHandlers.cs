namespace Producer.Features.PTN.Handlers;

public class YearHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Year, new Regex(@"[([]?[ .]?((?:19\d|20[012])\d[ .]?-[ .]?(?:19\d|20[012])\d)[ .]?[)\]]?"), Transformers.YearRange, new() { Remove = true });
        parser.AddHandler(ResultKeys.Year, new Regex(@"[([][ .]?((?:19\d|20[012])\d[ .]?-[ .]?\d{2})[ .]?[)\]]"), Transformers.YearRange, new() { Remove = true });
        parser.AddHandler(ResultKeys.Year, new Regex(@"[([]?(?!^)(?<!\d|Cap[. ]?)((?:19\d|20[012])\d)(?!\d|kbps)[)\]]?", RegexOptions.IgnoreCase), Transformers.Integer, new() { Remove = true });
        parser.AddHandler(ResultKeys.Year, new Regex(@"^[([]?((?:19\d|20[012])\d)(?!\d|kbps)[)\]]?", RegexOptions.IgnoreCase), Transformers.Integer, new() { Remove = true });
    }
}