namespace Producer.Features.PTN.Handlers;

public class DubbedHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Dubbed, new Regex(@"\b(?:DUBBED|dublado|dubbing|DUBS?)\b", RegexOptions.IgnoreCase), Transformers.Boolean, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Dubbed, Handler.CreateHandlerFromDelegate(ResultKeys.Dubbed, CustomDubbedHandlerOne), Transformers.None, ParseTorrentNameOptions.DefaultOptions);
    }

    private static Func<Dictionary<string, object>, HandlerResult> CustomDubbedHandlerOne
        => context =>
        {
            if (context.TryGetValue(ResultKeys.Result, out var resultObj) && resultObj is Dictionary<string, object> result)
            {
                if (result.TryGetValue(ResultKeys.Languages, out var languagesObj) && languagesObj is List<string> languages)
                {
                    if (new[] {"multi audio", "dual audio"}.Any(l => languages.Contains(l)))
                    {
                        result[ResultKeys.Dubbed] = true;
                    }
                }
            }

            return new() {MatchIndex = 0};
        };
}