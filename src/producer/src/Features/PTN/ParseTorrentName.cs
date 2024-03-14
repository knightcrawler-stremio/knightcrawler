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

            if (matchResult.Remove)
            {
                title = title.Remove(matchResult.MatchIndex, matchResult.RawMatch.Length);
            }

            if (!matchResult.SkipFromTitle && matchResult.MatchIndex < endOfTitle)
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
            Title = result.TryGetValue(ResultKeys.Title, out var titleValue) ? titleValue as string : null,
            Date = result.TryGetValue(ResultKeys.Date, out var dateValue) ? dateValue as string : null,
            Year = result.TryGetValue(ResultKeys.Year, out var yearValue) ? yearValue as string : null,
            Resolution = result.TryGetValue(ResultKeys.Resolution, out var resolutionValue) ? resolutionValue as string : null,
            Extended = result.TryGetValue(ResultKeys.Extended, out var extendedValue) && bool.TryParse(extendedValue as string, out var extendedBool) && extendedBool,
            Unrated = result.TryGetValue(ResultKeys.Unrated, out var unratedValue) && bool.TryParse(unratedValue as string, out var unratedBool) && unratedBool,
            Proper = result.TryGetValue(ResultKeys.Proper, out var properValue) && bool.TryParse(properValue as string, out var properBool) && properBool,
            Repack = result.TryGetValue(ResultKeys.Repack, out var repackValue) && bool.TryParse(repackValue as string, out var repackBool) && repackBool,
            Convert = result.TryGetValue(ResultKeys.Convert, out var convertValue) && bool.TryParse(convertValue as string, out var convertBool) && convertBool,
            Hardcoded = result.TryGetValue(ResultKeys.Hardcoded, out var hardcodedValue) && bool.TryParse(hardcodedValue as string, out var hardcodedBool) && hardcodedBool,
            Retail = result.TryGetValue(ResultKeys.Retail, out var retailValue) && bool.TryParse(retailValue as string, out var retailBool) && retailBool,
            Remastered = result.TryGetValue(ResultKeys.Remastered, out var remasteredValue) && bool.TryParse(remasteredValue as string, out var remasteredBool) && remasteredBool,
            Complete = result.TryGetValue(ResultKeys.Complete, out var completeValue) && bool.TryParse(completeValue as string, out var completeBool) && completeBool,
            Dubbed = result.TryGetValue(ResultKeys.Dubbed, out var dubbedValue) && bool.TryParse(dubbedValue as string, out var dubbedBool) && dubbedBool,
            Region = result.TryGetValue(ResultKeys.Region, out var regionValue) ? regionValue as string : null,
            Container = result.TryGetValue(ResultKeys.Container, out var containerValue) ? containerValue as string : null,
            Extension = result.TryGetValue(ResultKeys.Extension, out var extensionValue) ? extensionValue as string : null,
            Source = result.TryGetValue(ResultKeys.Source, out var sourceValue) ? sourceValue as string : null,
            Codec = result.TryGetValue(ResultKeys.Codec, out var codecValue) ? codecValue as string : null,
            BitDepth = result.TryGetValue(ResultKeys.BitDepth, out var bitDepthValue) ? bitDepthValue as string : null,
            Hdr = result.TryGetValue(ResultKeys.Hdr, out var hdrValue) ? hdrValue as List<string> : [],
            Audio = result.TryGetValue(ResultKeys.Audio, out var audioValue) ? audioValue as string : null,
            Group = result.TryGetValue(ResultKeys.Group, out var groupValue) ? groupValue as string : null,
            Volumes = result.TryGetValue(ResultKeys.Volumes, out var volumesValue) ? volumesValue as List<int> : [],
            Seasons = result.TryGetValue(ResultKeys.Seasons, out var seasonsValue) ? seasonsValue as List<int> : [],
            Episodes = result.TryGetValue(ResultKeys.Episodes, out var episodesValue) ? episodesValue as List<int> : [],
            EpisodeCode = result.TryGetValue(ResultKeys.EpisodeCode, out var episodeCodeValue) ? episodeCodeValue as string : null,
            Languages = result.TryGetValue(ResultKeys.Languages, out var languagesValue) ? languagesValue as string : null,
        };

        return parsedResult;
    }
}