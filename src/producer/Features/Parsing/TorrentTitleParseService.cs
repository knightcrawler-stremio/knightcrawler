namespace Producer.Features.Parsing;

using ParsedTorrent = TorrentTitleParser.Torrent;

public static partial class TorrentTitleParseService
{
    [GeneratedRegex(@"(season|episode)s?.?\d?", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SeasonEpisode();
    [GeneratedRegex(@"[se]\d\d", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SeasonShort();
    [GeneratedRegex(@"\b(tv|complete)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex TvOrComplete();
    [GeneratedRegex(@"\b(saison|stage).?\d", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SeasonStage();
    [GeneratedRegex(@"[a-z]\s?\-\s?\d{2,4}\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex Season();
    [GeneratedRegex(@"\d{2,4}\s?\-\s?\d{2,4}\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SeasonTwo();

    public static TorrentType GetTypeByName(string name)
    {
        var tvRegexes = new[]
        {
            SeasonEpisode,
            SeasonShort,
            TvOrComplete,
            SeasonStage,
            Season,
            SeasonTwo
        };

        foreach (var regex in tvRegexes)
        {
            if (regex().IsMatch(name))
            {
                return TorrentType.Tv;
            }
        }

        return TorrentType.Movie;
    }

    public static ParsedTorrent ParseTorrentName(string title) => new(title);
}
