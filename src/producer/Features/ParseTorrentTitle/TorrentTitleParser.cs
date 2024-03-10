namespace Producer.Features.ParseTorrentTitle;

public static partial class TorrentTitleParser
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

    public static ParsedFilename Parse(string name)
    {
        VideoCodecsParser.Parse(name, out var videoCodec, out _);
        AudioCodecsParser.Parse(name, out var audioCodec, out _);
        AudioChannelsParser.Parse(name, out var audioChannels, out _);
        LanguageParser.Parse(name, out var languages);
        QualityParser.Parse(name, out var quality);
        var group = GroupParser.Parse(name);
        var edition = EditionParser.Parse(name);
        var multi = LanguageParser.IsMulti(name);
        var complete = Complete.IsComplete(name);

        var baseParsed = new BaseParsed
        {
            Resolution = quality.Resolution,
            Sources = quality.Sources,
            VideoCodec = videoCodec,
            AudioCodec = audioCodec,
            AudioChannels = audioChannels,
            Revision = quality.Revision,
            Group = group,
            Edition = edition,
            Languages = languages,
            Multi = multi,
            Complete = complete,
        };

        var isTv = GetTypeByName(name) == TorrentType.Tv;

        return !isTv ? ParseMovie(name, baseParsed) : ParseSeason(name, baseParsed);
    }

    private static ParsedFilename ParseSeason(string name, BaseParsed baseParsed)
    {
        var season = SeasonParser.Parse(name);

        if (season == null)
        {
            return new();
        }

        return new()
        {
            Show = new()
            {
               EpisodeNumbers = season.EpisodeNumbers,
               FullSeason = season.FullSeason,
               IsPartialSeason = season.IsPartialSeason,
               IsSpecial = season.IsSpecial,
               SeasonPart = season.SeasonPart,
               IsSeasonExtra = season.IsSeasonExtra,
               SeriesTitle = season.SeriesTitle,
               IsMultiSeason = season.IsMultiSeason,
               AirDate = season.AirDate,
               Seasons = season.Seasons,
               ReleaseTitle = season.ReleaseTitle,
               Edition = baseParsed.Edition,
               Resolution = baseParsed.Resolution,
               Sources = baseParsed.Sources,
               VideoCodec = baseParsed.VideoCodec,
               Complete = baseParsed.Complete,
               AudioCodec = baseParsed.AudioCodec,
               Languages = baseParsed.Languages,
               AudioChannels = baseParsed.AudioChannels,
               Group = baseParsed.Group,
               Multi = baseParsed.Multi,
               Revision = baseParsed.Revision,
            },
        };
    }

    private static ParsedFilename ParseMovie(string name, BaseParsed baseParsed)
    {
       TitleParser.Parse(name, out var title, out var year);

       baseParsed.Title = title;
       baseParsed.Year = year;

       return new()
       {
           Movie = new()
           {
               Title = baseParsed.Title,
               Year = baseParsed.Year,
               Edition = baseParsed.Edition,
               Resolution = baseParsed.Resolution,
               Sources = baseParsed.Sources,
               VideoCodec = baseParsed.VideoCodec,
               Complete = baseParsed.Complete,
               AudioCodec = baseParsed.AudioCodec,
               Languages = baseParsed.Languages,
               AudioChannels = baseParsed.AudioChannels,
               Group = baseParsed.Group,
               Multi = baseParsed.Multi,
               Revision = baseParsed.Revision,
           },
       };
    }

    private static TorrentType GetTypeByName(string name)
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
}
