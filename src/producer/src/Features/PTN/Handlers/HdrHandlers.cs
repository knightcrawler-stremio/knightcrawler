namespace Producer.Features.PTN.Handlers;

public class HdrHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Hdr, new Regex(@"(HDR(?:10)?)", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value(ResultKeys.Hdr)), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Hdr, new Regex(@"(HDR10(?:\+|plus))", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("HDR10+")), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Hdr, new Regex(@"(\bDV\b|dolby.?vision|\bDoVi\b)", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("DV")), new() { Remove = true, SkipIfAlreadyFound = false });
    }
}