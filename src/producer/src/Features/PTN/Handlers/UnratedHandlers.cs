namespace Producer.Features.PTN.Handlers;

public class UnratedHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser) => parser.AddHandler(ResultKeys.Unrated, new Regex(@"\bunrated|uncensored\b", RegexOptions.IgnoreCase), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
}