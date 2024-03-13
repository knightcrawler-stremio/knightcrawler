namespace Producer.Features.PTN.Handlers;

public class SeasonHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:complete\W|seasons?\W|\W|^)((?:s\d{1,2}[., +/\\&-]+)+s\d{1,2}\b)", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:complete\W|seasons?\W|\W|^)[([]?(s\d{2,}-\d{2,}\b)[)\]]?", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:complete\W|seasons?\W|\W|^)[([]?(s[1-9]-[2-9]\b)[)\]]?", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:(?:\bthe\W)?\bcomplete\W)?(?:seasons?|[Сс]езони?|temporadas?)[. ]?[-:]?[. ]?[([]?((?:\d{1,2}[., /\\&]+)+\d{1,2}\b)[)\]]?", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:(?:\bthe\W)?\bcomplete\W)?(?:seasons|[Сс]езони?|temporadas?)[. ]?[-:]?[. ]?[([]?((?:\d{1,2}[. -]+)+[1-9]\d?\b)[)\]]?", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:(?:\bthe\W)?\bcomplete\W)?season[. ]?[([]?((?:\d{1,2}[. -]+)+[1-9]\d?\b)[)\]]?(?!.*\.\w{2,4}$)", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:(?:\bthe\W)?\bcomplete\W)?\bseasons?\b[. -]?(\d{1,2}[. -]?(?:to|thru|and|\+|:)[. -]?\d{1,2})\b", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:(?:\bthe\W)?\bcomplete\W)?(?:saison|seizoen|season|series|temp(?:orada)?):?[. ]?(\d{1,2})", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(\d{1,2})(?:-?й)?[. _]?(?:[Сс]езон|sez(?:on)?)(?:\W?\D|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"[Сс]езон:?[. _]?№?(\d{1,2})(?!\d)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:\D|^)(\d{1,2})Â?[°ºªa]?[. ]*temporada", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), new() { Remove = true });
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"t(\d{1,3})(?:[ex]+|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), new() { Remove = true });
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:(?:\bthe\W)?\bcomplete)?(?:\W|^)s(\d{1,3})(?:[\Wex]|\d{2}\b|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), new() { SkipIfAlreadyFound = false });
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:(?:\bthe\W)?\bcomplete)?(?:\W|^)(\d{1,2})[. ]?(?:st|nd|rd|th)[. ]*season", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:\D|^)(\d{1,2})[xх]\d{1,3}(?:\D|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"\bSn([1-9])(?:\D|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"[[(](\d{1,2})\.\d{1,3}[)\]]", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"-\s?(\d{1,2})\.\d{2,3}\s?-", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?:^|\/)(\d{1,2})-\d{2}\b(?!-\d)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"[^\w-](\d{1,2})-\d{2}(?=\.\w{2,4}$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"(?<!\bEp?(?:isode)? ?\d+\b.*)\b(\d{2})[ ._]\d{2}(?:.F)?\.\w{2,4}$", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.Seasons, new Regex(@"\bEp(?:isode)?\W+(\d{1,2})\.\d{1,3}\b", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), ParseTorrentNameOptions.DefaultOptions);
    }
}