namespace Producer.Features.PTN.Handlers;

public class VolumesHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Volumes, new Regex(@"\bvol(?:s|umes?)?[. -]*(?:\d{1,2}[., +/\\&-]+)+\d{1,2}\b", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler(ResultKeys.Volumes, Handler.CreateHandlerFromDelegate(ResultKeys.Volumes, CustomVolumesHandlerOne), Transformers.Range, new() { Remove = true });
    }

    private static Func<Dictionary<string, object>, HandlerResult> CustomVolumesHandlerOne
        => context =>
        {
            var title = context[ResultKeys.Title] as string;
            var result = context[ResultKeys.Result] as Dictionary<string, object>;
            var matched = context[ResultKeys.Matched] as Dictionary<string, object>;

            var startIndex = matched.ContainsKey(ResultKeys.Year) && matched[ResultKeys.Year] is Match yearMatch ? yearMatch.Index : 0;
            var match = Regex.Match(title[startIndex..], @"\bvol(?:ume)?[. -]*(\d{1,2})", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                matched[ResultKeys.Volumes] = new
                {
                    match = match.Value, matchIndex = match.Index
                };
                result[ResultKeys.Volumes] = Transformers.Array(Transformers.Integer)(match.Value, match.Groups[1].Value);
                return new()
                {
                    RawMatch = match.Value, MatchIndex = match.Index, Remove = true
                };
            }

            return null;
        };
}