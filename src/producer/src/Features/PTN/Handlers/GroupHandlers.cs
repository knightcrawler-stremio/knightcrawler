namespace Producer.Features.PTN.Handlers;

public class GroupHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Group, new Regex(@"- ?(?!\d+$|S\d+|\d+x|ep?\d+|[^[]+]$)([^\-. []+[^\-. [)\]\d][^\-. [)\]]*)(?:\[[\w.-]+])?(?=\.\w{2,4}$|$)", RegexOptions.IgnoreCase), Transformers.None, new() {Remove = true});
        parser.AddHandler(ResultKeys.Group, new Regex(@"^\[((?!DVD)[^\[\]]+)]", RegexOptions.IgnoreCase), Transformers.None, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Group, new Regex(@"\(([\w-]+)\)(?:$|\.\w{2,4}$)", RegexOptions.IgnoreCase), Transformers.None, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Group, Handler.CreateHandlerFromDelegate(ResultKeys.Group, AnimeMatch), Transformers.None, ParseTorrentNameOptions.DefaultOptions);
    }
    
    private static Func<Dictionary<string, object>, HandlerResult> AnimeMatch
        => context =>
        {
            var result = context[ResultKeys.Result] as Dictionary<string, object>;
            var matched = context[ResultKeys.Matched] as Dictionary<string, object>;

            if (matched.TryGetValue(ResultKeys.Group, out var groupValue))
            {
                var groupMatch = groupValue as string;
                if (!string.IsNullOrEmpty(groupMatch) && Regex.IsMatch(groupMatch, @"^\[.+]$"))
                {
                    var endIndex = groupMatch.Length;

                    if (matched.Keys.Any(key =>
                        {
                            if (matched[key] is string otherMatch)
                            {
                                return otherMatch.Length < endIndex;
                            }

                            return false;
                        }))
                    {
                        result.Remove(ResultKeys.Group);
                    }
                }
            }

            return new() { MatchIndex = 0 };
        };
}