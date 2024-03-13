namespace Producer.Features.PTN;

public class PTN : IPTN
{
    private readonly List<Handler> _handlers = [];

    public void AddHandler(string? handlerName, object? handler, Func<string, string, string>? transformer, Options options)
    {
        if (handler is Handler functionHandler)
        {
            functionHandler.HandlerName = handlerName ?? "unknown";
            _handlers.Add(functionHandler);
        }
        else if (!handlerName.IsNullOrEmpty() && handler is Regex regexHandler)
        {
            transformer ??= (_, value) => value;
            options = Options.ExtendOptions(options);
            var newHandler = Handler.CreateHandlerFromRegExp(handlerName, regexHandler, transformer, options);
            _handlers.Add(newHandler);
        }
        else
        {
            throw new ArgumentException($"Handler for {handlerName} should be a RegExp or a function. Got: {handler?.GetType().Name}");
        }
    }

    public Dictionary<string, object> Parse(string title)
    {
        title = title.Replace("_", " ", StringComparison.OrdinalIgnoreCase);
        var result = new Dictionary<string, object>();
        var matched = new Dictionary<string, object>();
        var endOfTitle = title.Length;

        foreach (var handler in _handlers)
        {
            var matchResult = handler.Function(new() {{"title", title}, {"result", result}, {"matched", matched}});

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

        result["title"] = title.CleanTitle()[..endOfTitle];

        return result;
    }
}
