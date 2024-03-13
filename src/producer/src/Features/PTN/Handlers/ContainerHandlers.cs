namespace Producer.Features.PTN.Handlers;

public class ContainerHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser) => parser.AddHandler(ResultKeys.Container, new Regex(@"\.?[[(]?\b(MKV|AVI|MP4|WMV|MPG|MPEG)\b[\])]?", RegexOptions.IgnoreCase), Transformers.Lowercase, ParseTorrentNameOptions.DefaultOptions);
}