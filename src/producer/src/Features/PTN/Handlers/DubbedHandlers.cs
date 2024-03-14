namespace Producer.Features.PTN.Handlers;

public class DubbedHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser) => 
        parser.AddHandler(ResultKeys.Dubbed, new Regex(@"(\b(?:DUBBED|dublado|dubbing|DUBS?|Dual(-Audio)?|Multi[-\s]Audio?)\b)(?!-Subs)", RegexOptions.IgnoreCase), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);

}