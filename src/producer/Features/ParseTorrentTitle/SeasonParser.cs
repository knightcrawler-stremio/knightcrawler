namespace Producer.Features.ParseTorrentTitle;

public static partial class SeasonParser
{
    [GeneratedRegex(@"^(?:\[.+?\])+", RegexOptions.None)]
    private static partial Regex RequestInfoExp();

    [GeneratedRegex(@"(?<=[_.-])(?<airdate>(?<!\d)(?<airyear>[1-9]\d{1})(?<airmonth>[0-1][0-9])(?<airday>[0-3][0-9]))(?=[_.-])", RegexOptions.IgnoreCase)]
    private static partial Regex SixDigitAirDateMatchExp();

    public static Season? Parse(string title)
    {
        if (!PreValidation(title))
        {
            return null;
        }

        var simpleTitle = TitleParser.SimplifyTitle(title);

        // parse daily episodes with mmddyy eg `At.Midnight.140722.720p.HDTV.x264-YesTV`
        var sixDigitAirDateMatch = SixDigitAirDateMatchExp().Match(title);

        if (sixDigitAirDateMatch.Groups.Count > 0)
        {
            var airYear = sixDigitAirDateMatch.Groups["airyear"]?.Value ?? "";
            var airMonth = sixDigitAirDateMatch.Groups["airmonth"]?.Value ?? "";
            var airDay = sixDigitAirDateMatch.Groups["airday"]?.Value ?? "";

            if (airMonth != "00" || airDay != "00")
            {
                var fixedDate = $"20{airYear}.{airMonth}.{airDay}";

                if (!string.IsNullOrEmpty(sixDigitAirDateMatch.Groups["airdate"].Value))
                {
                    simpleTitle = simpleTitle.Replace(sixDigitAirDateMatch.Groups["airdate"].Value, fixedDate);
                }
            }
        }

        foreach (var exp in _validRegexes)
        {
            var match = exp().Match(simpleTitle);

            if (match.Groups.Count <= 0 || !match.Success)
            {
                continue;
            }

            var result = ParseMatchCollection(match, simpleTitle);

            if (result.FullSeason && result.ReleaseTokens != null && result.ReleaseTokens.Contains("Special", StringComparison.OrdinalIgnoreCase))
            {
                result.FullSeason = false;
                result.IsSpecial = true;
            }

            return new()
            {
                ReleaseTitle = title,
                SeriesTitle = result.SeriesName,
                Seasons = result.SeasonNumbers ?? [],
                EpisodeNumbers = result.EpisodeNumbers ?? [],
                AirDate = result.AirDate,
                FullSeason = result.FullSeason,
                IsPartialSeason = result.IsPartialSeason ?? false,
                IsMultiSeason = result.IsMultiSeason ?? false,
                IsSeasonExtra = result.IsSeasonExtra ?? false,
                IsSpecial = result.IsSpecial ?? false,
                SeasonPart = result.SeasonPart ?? 0,
            };
        }

        return null;
    }

    private static ParsedMatch ParseMatchCollection(Match match, string simpleTitle)
    {
        var groups = match.Groups;

        if (groups.Count == 0)
        {
            throw new("No match");
        }

        var seriesName = groups["title"].Value
            .Replace(".", " ")
            .Replace("_", " ")
            .Replace(RequestInfoExp().ToString(), "")
            .Trim();

        var result = new ParsedMatch
        {
            SeriesName = seriesName,
        };

        var lastSeasonEpisodeStringIndex = IndexOfEnd(simpleTitle, groups["title"].Value);

        if (int.TryParse(groups["airyear"].Value, out var airYear) && airYear >= 1900)
        {
            var seasons = new List<string> {groups["season"]?.Value, groups["season1"]?.Value}
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(
                    x =>
                    {
                        lastSeasonEpisodeStringIndex = Math.Max(
                            IndexOfEnd(simpleTitle, x ?? ""),
                            lastSeasonEpisodeStringIndex
                        );
                        return int.Parse(x);
                    })
                .ToList();

            if (seasons.Count > 1)
            {
                seasons = CompleteRange(seasons);
            }

            result.SeasonNumbers = seasons;

            if (seasons.Count > 1)
            {
                result.IsMultiSeason = true;
            }

            var episodeCaptures = new List<string> {groups["episode"]?.Value, groups["episode1"]?.Value}
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            var absoluteEpisodeCaptures = new List<string> {groups["absoluteepisode"]?.Value, groups["absoluteepisode1"]?.Value}
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            // handle 0 episode possibly indicating a full season release
            if (episodeCaptures.Any())
            {
                var first = int.Parse(episodeCaptures[0]);
                var last = int.Parse(episodeCaptures[^1]);

                if (first > last)
                {
                    return null;
                }

                var count = last - first + 1;
                result.EpisodeNumbers = Enumerable.Range(first, count).ToList();
            }

            if (absoluteEpisodeCaptures.Any())
            {
                var first = double.Parse(absoluteEpisodeCaptures[0]);
                var last = double.Parse(absoluteEpisodeCaptures[^1]);

                if (first % 1 != 0 || last % 1 != 0)
                {
                    if (absoluteEpisodeCaptures.Count != 1)
                    {
                        return null;
                    }

                    // specialAbsoluteEpisodeNumbers in radarr
                    result.EpisodeNumbers = new()
                        {(int) first};
                    result.IsSpecial = true;

                    lastSeasonEpisodeStringIndex = Math.Max(
                        IndexOfEnd(simpleTitle, absoluteEpisodeCaptures[0] ?? ""),
                        lastSeasonEpisodeStringIndex
                    );
                }
                else
                {
                    var count = (int) (last - first + 1);
                    // AbsoluteEpisodeNumbers in radarr
                    result.EpisodeNumbers = Enumerable.Range((int) first, count).ToList();

                    if (groups["special"]?.Value != null)
                    {
                        result.IsSpecial = true;
                    }
                }
            }

            if (!episodeCaptures.Any() && !absoluteEpisodeCaptures.Any())
            {
                // Check to see if this is an "Extras" or "SUBPACK" release, if it is, set
                // IsSeasonExtra so they can be filtered out
                if (groups["extras"]?.Value != null)
                {
                    result.IsSeasonExtra = true;
                }

                // Partial season packs will have a seasonpart group so they can be differentiated
                // from a full season/single episode release
                var seasonPart = groups["seasonpart"]?.Value;

                if (seasonPart != null)
                {
                    result.SeasonPart = int.Parse(seasonPart);
                    result.IsPartialSeason = true;
                }
                else
                {
                    result.FullSeason = true;
                }
            }

            if (absoluteEpisodeCaptures.Any() && result.EpisodeNumbers == null)
            {
                result.SeasonNumbers = new()
                    {0};
            }
        }
        else
        {
            if (int.TryParse(groups["airmonth"]?.Value, out var airMonth) && int.TryParse(groups["airday"]?.Value, out var airDay))
            {
                // Swap day and month if month is bigger than 12 (scene fail)
                if (airMonth > 12)
                {
                    (airDay, airMonth) = (airMonth, airDay);
                }

                var airDate = new DateTime(airYear, airMonth, airDay);

                // dates in the future is most likely parser error
                if (airDate > DateTime.Now)
                {
                    throw new("Parsed date is in the future");
                }

                if (airDate < new DateTime(1970, 1, 1))
                {
                    throw new("Parsed date error");
                }

                lastSeasonEpisodeStringIndex = Math.Max(
                    IndexOfEnd(simpleTitle, groups["airyear"]?.Value ?? ""),
                    lastSeasonEpisodeStringIndex
                );
                lastSeasonEpisodeStringIndex = Math.Max(
                    IndexOfEnd(simpleTitle, groups["airmonth"]?.Value ?? ""),
                    lastSeasonEpisodeStringIndex
                );
                lastSeasonEpisodeStringIndex = Math.Max(
                    IndexOfEnd(simpleTitle, groups["airday"]?.Value ?? ""),
                    lastSeasonEpisodeStringIndex
                );
                result.AirDate = airDate;
            }
        }

        if (lastSeasonEpisodeStringIndex == simpleTitle.Length || lastSeasonEpisodeStringIndex == -1)
        {
            result.ReleaseTokens = simpleTitle;
        }
        else
        {
            result.ReleaseTokens = simpleTitle.Substring(lastSeasonEpisodeStringIndex);
        }

        result.SeriesTitle = seriesName;
        // TODO: seriesTitleInfo

        return result;
    }

    private static bool PreValidation(string title) =>
        _rejectedRegex.Select(exp => exp().Match(title)).All(match => !match.Success);

    private static List<int> CompleteRange(List<int> arr)
    {
        var uniqArr = arr.Distinct().ToList();

        var first = uniqArr[0];
        var last = uniqArr[^1];

        if (first > last)
        {
            return arr;
        }

        var count = last - first + 1;
        return Enumerable.Range(first, count).ToList();
    }

    private static int IndexOfEnd(string str1, string str2)
    {
        var io = str1.IndexOf(str2, StringComparison.Ordinal);
        return io == -1 ? -1 : io + str2.Length;
    }

    private record ParsedMatch
    {
        public string? SeriesName { get; set; }
        public string? SeriesTitle { get; set; }
        public List<int>? SeasonNumbers { get; set; }
        public bool? IsMultiSeason { get; set; }
        public List<int>? EpisodeNumbers { get; set; }
        public bool? IsSpecial { get; set; }
        public bool? IsSeasonExtra { get; set; }
        public int? SeasonPart { get; set; }
        public bool? IsPartialSeason { get; set; }
        public bool FullSeason { get; set; }
        public DateTime? AirDate { get; set; }
        public string? ReleaseTokens { get; set; }
    }
}
