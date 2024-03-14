namespace Producer.Features.PTN;

public class ParseTorrentName : IParseTorrentName
{
    private readonly List<Handler> _handlers = [];

    private static Func<Regex>[] _tvRegexes { get; set; } =
    [
        ParserRegex.SeasonEpisode,
        ParserRegex.SeasonShort,
        ParserRegex.TvOrComplete,
        ParserRegex.SeasonStage,
        ParserRegex.Season,
        ParserRegex.SeasonTwo,
    ];

    public ParseTorrentName(IEnumerable<IPtnHandler> handlers)
    {
        foreach (var handler in handlers)
        {
            handler.RegisterHandlers(this);
        }
    }

    public void AddHandler(string? handlerName, object? handler, Func<string, string, string>? transformer, ParseTorrentNameOptions options)
    {
        if (handler is Handler functionHandler)
        {
            functionHandler.HandlerName = handlerName ?? "unknown";
            _handlers.Add(functionHandler);
        }
        else if (!handlerName.IsNullOrEmpty() && handler is Regex regexHandler)
        {
            transformer ??= (_, value) => value;
            options = ParseTorrentNameOptions.ExtendOptions(options);
            var newHandler = Handler.CreateHandlerFromRegExp(handlerName, regexHandler, transformer, options);
            _handlers.Add(newHandler);
        }
        else
        {
            throw new ArgumentException($"Handler for {handlerName} should be a RegExp or a function. Got: {handler?.GetType().Name}");
        }
    }
    
    public TorrentType GetTorrentTypeByName(string name) => _tvRegexes.Any(regex => regex().IsMatch(name)) ? TorrentType.Tv : TorrentType.Movie;

    public ParsedTorrentResult Parse(string title)
    {
        title = title.Replace("_", " ", StringComparison.OrdinalIgnoreCase);
        var result = new Dictionary<string, object>();
        var matched = new Dictionary<string, object>();
        var endOfTitle = title.Length;

        foreach (var handler in _handlers)
        {
            var matchResult = handler.Function(new() {{ResultKeys.Title, title}, {ResultKeys.Result, result}, {ResultKeys.Matched, matched}});
            
            if (matchResult is null)
            {
                continue;
            }

            if (matchResult.Remove == true)
            {
                title = title.Remove(matchResult.MatchIndex, matchResult.RawMatch.Length);
            }

            if (!matchResult.SkipFromTitle == true && matchResult.MatchIndex < endOfTitle)
            {
                endOfTitle = matchResult.MatchIndex;
            }

            if (matchResult is {Remove: true, SkipFromTitle: true} && matchResult.MatchIndex < endOfTitle)
            {
                endOfTitle -= matchResult.RawMatch.Length;
            }
        }

        result[ResultKeys.Title] = title.CleanTitle();

        var parsedResult = new ParsedTorrentResult
        {
            Title = result.GetValueAsString(ResultKeys.Title),
            Date = result.GetValueAsString(ResultKeys.Date),
            Year = result.GetValueAsString(ResultKeys.Year),
            Resolution = result.GetValueAsString(ResultKeys.Resolution),
            Region = result.GetValueAsString(ResultKeys.Region),
            Container = result.GetValueAsString(ResultKeys.Container),
            Extension = result.GetValueAsString(ResultKeys.Extension),
            Source = result.GetValueAsString(ResultKeys.Source),
            Codec = result.GetValueAsString(ResultKeys.Codec),
            BitDepth = result.GetValueAsString(ResultKeys.BitDepth),
            Audio = result.GetValueAsString(ResultKeys.Audio),
            Group = result.GetValueAsString(ResultKeys.Group),
            EpisodeCode = result.GetValueAsString(ResultKeys.EpisodeCode),
            Languages = result.GetValueAsString(ResultKeys.Languages),
            
            Extended = result.GetValueAsBool(ResultKeys.Extended),
            Unrated = result.GetValueAsBool(ResultKeys.Unrated),
            Proper = result.GetValueAsBool(ResultKeys.Proper),
            Repack = result.GetValueAsBool(ResultKeys.Repack),
            Convert = result.GetValueAsBool(ResultKeys.Convert),
            Hardcoded = result.GetValueAsBool(ResultKeys.Hardcoded),
            Retail = result.GetValueAsBool(ResultKeys.Retail),
            Remastered = result.GetValueAsBool(ResultKeys.Remastered),
            Complete = result.GetValueAsBool(ResultKeys.Complete),
            Dubbed = result.GetValueAsBool(ResultKeys.Dubbed),
            
            Hdr = result.GetValueAsList<string>(ResultKeys.Hdr),
            Volumes = result.TryGetValue(ResultKeys.Volumes, out var volumesValue) ? volumesValue as List<int> : [],
            Seasons = result.TryGetValue(ResultKeys.Seasons, out var seasonsValue) ? seasonsValue as List<int> : [],
            Episodes = result.TryGetValue(ResultKeys.Episodes, out var episodesValue) ? episodesValue as List<int> : [],
            
        };

        return parsedResult;
    }
}