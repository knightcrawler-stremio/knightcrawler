namespace Producer.Features.PTN;

public class Handler
{
    public string HandlerName { get; set; } = default!;

    public Func<Dictionary<string, object>, HandlerResult> Function { get; set; } = default!;

    public static Handler CreateHandlerFromDelegate(string name, Func<Dictionary<string, object>, HandlerResult> function) =>
        new()
        {
            HandlerName = name,
            Function = function,
        };

    public static Handler CreateHandlerFromRegExp(string name, Regex regExp, Func<string, string, string>? transformer, Options options)
    {
        HandlerResult Func(Dictionary<string, object> context)
        {
            var title = context["title"] as string;
            var result = context["result"] as Dictionary<string, object>;
            var matched = context["matched"] as Dictionary<string, object>;

            if (result.ContainsKey(name) && options.SkipIfAlreadyFound)
            {
                return null;
            }

            var match = regExp.Match(title);
            var rawMatch = match.Value;
            var cleanMatch = match.Groups[1].Value;

            if (!string.IsNullOrEmpty(rawMatch))
            {
                var transformed = transformer(cleanMatch, result.TryGetValue(name, out var value) ? value as string : null);
                var beforeTitleMatch = ParserRegex.BeforeTitle().Match(title);
                var isBeforeTitle = beforeTitleMatch.Success && beforeTitleMatch.Groups[1].Value.Contains(rawMatch, StringComparison.OrdinalIgnoreCase);
                var otherMatches = matched.Where(e => e.Key != name).ToList();
                var isSkipIfFirst = options.SkipIfFirst && otherMatches.Count > 0 && otherMatches.All(e => match.Index < (int) e.Value);

                if (!string.IsNullOrEmpty(transformed) && !isSkipIfFirst)
                {
                    if (!matched.ContainsKey(name))
                    {
                        matched[name] = new
                        {
                            rawMatch, matchIndex = match.Index
                        };
                    }

                    result[name] = options.Value ?? transformed;

                    return new()
                    {
                        RawMatch = rawMatch, MatchIndex = match.Index, Remove = options.Remove, SkipFromTitle = isBeforeTitle || options.SkipFromTitle
                    };
                }
            }

            return null;
        }

        return new()
        {
            HandlerName = name,
            Function = Func,
        };
    }
}
