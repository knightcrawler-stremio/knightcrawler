using System.Web;

namespace Producer.Features.ParseTorrentTitle;

public static partial class TorrentTitleParser
{
    public static Func<Regex>[] TvRegexes { get; private set; } =
    [
        SeasonEpisode,
        SeasonShort,
        TvOrComplete,
        SeasonStage,
        Season,
        SeasonTwo,
    ];

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

    public static TorrentType GetTypeByName(string name) =>
        TvRegexes.Any(regex => regex().IsMatch(name)) ? TorrentType.Tv : TorrentType.Movie;

    public static ParsedTorrent Parse(string title) => ParsedTorrent.ParseInfo(title);
}
