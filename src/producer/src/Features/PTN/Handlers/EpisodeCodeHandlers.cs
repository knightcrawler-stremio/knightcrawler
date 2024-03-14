namespace Producer.Features.PTN.Handlers;

public class EpisodeCodeHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.EpisodeCode, new Regex(@"\[(?<episodeCode>[a-zA-Z0-9]{8})\](?=\.[a-zA-Z0-9]{1,5}$|$)"), Transformers.Uppercase, new() { Remove = true });
        parser.AddHandler(ResultKeys.EpisodeCode, new Regex(@"\[(?<episodeCode>[A-Z0-9]{8})\]"), Transformers.Uppercase, new() { Remove = true });
        parser.AddHandler(ResultKeys.EpisodeCode, new Regex(@"\((?<episodeCode>[A-Z0-9]{8})\)"), Transformers.Uppercase, new() { Remove = true });
    }
}