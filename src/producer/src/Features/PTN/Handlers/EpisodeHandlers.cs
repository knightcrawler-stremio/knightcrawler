namespace Producer.Features.PTN.Handlers;

public class EpisodeHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?:[\W\d]|^)e[ .]?[([]?(\d{1,3}(?:[ .-]*(?:[&+]|e){1,2}[ .]?\d{1,3})+)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?:[\W\d]|^)ep[ .]?[([]?(\d{1,3}(?:[ .-]*(?:[&+]|ep){1,2}[ .]?\d{1,3})+)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?:[\W\d]|^)\d+[xх][ .]?[([]?(\d{1,3}(?:[ .]?[xх][ .]?\d{1,3})+)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?:[\W\d]|^)(?:episodes?|[Сс]ерии:?)[ .]?[([]?(\d{1,3}(?:[ .+]*[&+][ .]?\d{1,3})+)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"[([]?(?:\D|^)(\d{1,3}[ .]?ao[ .]?\d{1,3})[)\]]?(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?:[\W\d]|^)(?:e|eps?|episodes?|[Сс]ерии:?|\d+[xх])[ .]*[([]?(\d{1,3}(?:-\d{1,3})+)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?:\W|^)[st]\d{1,2}[. ]?[xх-]?[. ]?(?:e|x|х|ep|-|\.)[. ]?(\d{1,3})(?:[abc]|v0?[1-4]|\D|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"\b[st]\d{2}(\d{2})\b", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?:\W|^)(\d{1,3}(?:[ .]*~[ .]*\d{1,3})+)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"-\s(\d{1,3}[ .]*-[ .]*\d{1,3})(?!-\d)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"s\d{1,2}\s?\((\d{1,3}[ .]*-[ .]*\d{1,3})\)", RegexOptions.IgnoreCase), Transformers.Range, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?:^|\/)\d{1,2}-(\d{2})\b(?!-\d)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?<!\d-)\b\d{1,2}-(\d{2})(?=\.\w{2,4}$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?<!(?:seasons?|[Сс]езони?)\W*)(?:[ .([-]|^)(\d{1,3}(?:[ .]?[,&+~][ .]?\d{1,3})+)(?:[ .)\]-]|$)", RegexOptions.IgnoreCase), Transformers.Range, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?<!(?:seasons?|[Сс]езони?)\W*)(?:[ .([-]|^)(\d{1,3}(?:-\d{1,3})+)(?:[ .)(\]]|-\D|$)", RegexOptions.IgnoreCase), Transformers.Range, ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"\bEp(?:isode)?\W+\d{1,2}\.(\d{1,3})\b", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?:\b[ée]p?(?:isode)?|[Ээ]пизод|[Сс]ер(?:ии|ия|\.)?|cap(?:itulo)?|epis[oó]dio)[. ]?[-:#№]?[. ]?(\d{1,4})(?:[abc]|v0?[1-4]|\W|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"\b(\d{1,3})(?:-?я)?[ ._-]*(?:ser(?:i?[iyj]a|\b)|[Сс]ер(?:ии|ия|\.)?)/i"), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?:\D|^)\d{1,2}[. ]?[xх][. ]?(\d{1,3})(?:[abc]|v0?[1-4]|\D|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"[[(]\d{1,2}\.(\d{1,3})[)\]]", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"\b[Ss]\d{1,2}[ .](\d{1,2})\b", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"-\s?\d{1,2}\.(\d{2,3})\s?-/", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?<=\D|^)(\d{1,3})[. ]?(?:of|из|iz)[. ]?\d{1,3}(?=\D|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"\b\d{2}[ ._-](\d{2})(?:.F)?\.\w{2,4}$", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, new Regex(@"(?<!^)\[(\d{2,3})](?!(?:\.\w{2,4})?$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Episodes, Handler.CreateHandlerFromDelegate(ResultKeys.Episodes, CustomEpisodeHandlerOne), Transformers.None, ParseTorrentNameOptions.DefaultOptions);
    }

    private static Func<Dictionary<string, object>, HandlerResult> CustomEpisodeHandlerOne
        => context =>
        {
            var title = context[ResultKeys.Title] as string;
            var result = context[ResultKeys.Result] as Dictionary<string, object>;
            var matched = context[ResultKeys.Matched] as Dictionary<string, object>;

            if (!result.ContainsKey(ResultKeys.Episodes))
            {
                var startIndexes = new List<int>
                    {matched.TryGetValue(ResultKeys.Year, out var value) ? (int) value : 0, matched.TryGetValue(ResultKeys.Seasons, out var value1) ? (int) value1 : 0};
                var endIndexes = new List<int>
                {
                    matched.TryGetValue(ResultKeys.Resolution, out var value2) ? (int) value2 : title.Length,
                    matched.TryGetValue(ResultKeys.Source, out var value3) ? (int) value3 : title.Length,
                    matched.TryGetValue(ResultKeys.Codec, out var value4) ? (int) value4 : title.Length,
                    matched.TryGetValue(ResultKeys.Audio, out var value5) ? (int) value5 : title.Length
                };
                var startIndex = startIndexes.Min();
                var endIndex = endIndexes.Min();
                var beginningTitle = title.Substring(0, endIndex);
                var middleTitle = title.Substring(startIndex, endIndex - startIndex);

                var match1 = Regex.Match(
                    beginningTitle, @"(?<!movie\W*|film\W*|^)(?:[ .]+-[ .]+|[([][ .]*)(\d{1,4})(?:a|b|v\d)?(?:\W|$)(?!movie|film)",
                    RegexOptions.IgnoreCase);
                var match2 = Regex.Match(middleTitle, @"^(?:[([-][ .]?)?(\d{1,4})(?:a|b|v\d)?(?:\W|$)(?!movie|film)", RegexOptions.IgnoreCase);

                if (match1.Success)
                {
                    result[ResultKeys.Episodes] = new List<int> {int.Parse(match1.Groups[^1].Value)};
                    return new() {MatchIndex = title.IndexOf(match1.Value, StringComparison.OrdinalIgnoreCase)};
                }

                if (match2.Success)
                {
                    result[ResultKeys.Episodes] = new List<int> {int.Parse(match2.Groups[^1].Value)};
                    return new() {MatchIndex = title.IndexOf(match2.Value, StringComparison.OrdinalIgnoreCase)};
                }
            }

            return null;
        };
}