namespace Producer.Features.ParseTorrentTitle;

public static partial class SeasonParser
{
    [GeneratedRegex(@"^(?<airyear>19[6-9]\d|20\d\d)(?<sep>[-_]?)(?<airmonth>0\d|1[0-2])\k<sep>(?<airday>[0-2]\d|3[01])(?!\d)", RegexOptions.IgnoreCase)]
    private static partial Regex DailyEpisodesWithoutTitleExp();

    [GeneratedRegex(@"^(?:\W*S?(?<season>(?<!\d+)(?:\d{1,2}|\d{4})(?!\d+))(?:(?:[ex]){1,2}(?<episode>\d{1,3}(?!\d+)))+){2,}", RegexOptions.IgnoreCase)]
    private static partial Regex MultiPartEpisodesWithoutTitleExp();

    [GeneratedRegex(@"^(?<title>.+?)[-_. ]S(?<season>(?<!\d+)(?:\d{1,2}|\d{4})(?!\d+))(?:[E-_. ]?[ex]?(?<episode>(?<!\d+)\d{1,2}(?!\d+)))+(?:[-_. ]?[ex]?(?<episode1>(?<!\d+)\d{1,2}(?!\d+)))+", RegexOptions.IgnoreCase)]
    private static partial Regex MultiEpisodeWithSingleEpisodeNumbersExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:(?:[-_\W](?<![()[!]))+S?(?<season>(?<!\d+)(?:\d{1,2})(?!\d+))(?:[ex]|\W[ex]|_){1,2}(?<episode>\d{2,3}(?!\d+))(?:(?:-|[ex]|\W[ex]|_){1,2}(?<episode1>\d{2,3}(?!\d+)))+).+?(?:\[.+?\])(?!\\)", RegexOptions.IgnoreCase)]
    private static partial Regex MultiEpisodeWithTitleAndTrailingInfoInSlashesExp();

    [GeneratedRegex(@"(?:S?(?<season>(?<!\d+)(?:\d{1,2}|\d{4})(?!\d+))(?:(?:[-_]|[ex]){1,2}(?<episode>\d{2,3}(?!\d+))){2,})", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodesWithoutTitleMultiExp();

    [GeneratedRegex(@"^(?:S?(?<season>(?<!\d+)(?:\d{1,2}|\d{4})(?!\d+))(?:(?:[-_ ]?[ex])(?<episode>\d{2,3}(?!\d+))))", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodesWithoutTitleSingleExp();

    [GeneratedRegex(@"^(?:\[(?<subgroup>.+?)\][-_. ]?)(?<title>.+?)[-_. ](?:Episode)(?:[-_. ]+(?<absoluteepisode>(?<!\d+)\d{2,3}(\.\d{1,2})?(?!\d+)))+(?:_|-|\s|\.)*?(?<hash>\[.{8}\])?(?:$|\.)?", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeSubGroupTitleEpisodeAbsoluteEpisodeNumberExp();

    [GeneratedRegex(@"^(?:\[(?<subgroup>.+?)\](?:_|-|\s|\.)?)(?<title>.+?)(?:(?:[-_\W](?<![()[!]))+(?<absoluteepisode>\d{2,3}(\.\d{1,2})?))+(?:_|-|\s|\.)+(?:S?(?<season>(?<!\d+)\d{1,2}(?!\d+))(?:(?:-|[ex]|\W[ex]){1,2}(?<episode>\d{2}(?!\d+)))+).*?(?<hash>[([]\w{8}[)\]])?(?:$|\.)", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeSubGroupTitleAbsoluteEpisodeNumberSeasonEpisodeExp();

    [GeneratedRegex(@"^(?:\[(?<subgroup>.+?)\](?:_|-|\s|\.)?)(?<title>.+?)(?:[-_\W](?<![()[!]))+(?:S?(?<season>(?<!\d+)\d{1,2}(?!\d+))(?:(?:[ex]|\W[ex]){1,2}(?<episode>\d{2}(?!\d+)))+)(?:(?:_|-|\s|\.)+(?<absoluteepisode>(?<!\d+)\d{2,3}(\.\d{1,2})?(?!\d+)))+.*?(?<hash>\[\w{8}\])?(?:$|\.)", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeSubGroupTitleSeasonEpisodeAbsoluteEpisodeNumberExp();

    [GeneratedRegex(@"^(?:\[(?<subgroup>.+?)\](?:_|-|\s|\.)?)(?<title>.+?)(?:[-_\W](?<![()[!]))+(?:S?(?<season>(?<!\d+)\d{1,2}(?!\d+))(?:(?:[ex]|\W[ex]){1,2}(?<episode>\d{2}(?!\d+)))+)(?:\s|\.).*?(?<hash>\[\w{8}\])?(?:$|\.)", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeSubGroupTitleSeasonEpisodeExp();

    [GeneratedRegex(@"^\[(?<subgroup>.+?)\][-_. ]?(?<title>[^-]+?\d+?)[-_. ]+(?:[-_. ]?(?<absoluteepisode>\d{3}(\.\d{1,2})?(?!\d+)))+(?:[-_. ]+(?<special>special|ova|ovd))?.*?(?<hash>\[\w{8}\])?(?:$|\.mkv)", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeSubGroupTitleWithTrailingNumberAbsoluteEpisodeNumberExp();

    [GeneratedRegex(@"^\[(?<subgroup>.+?)\][-_. ]?(?<title>.+?)(?:[. ]-[. ](?<absoluteepisode>\d{2,3}(\.\d{1,2})?(?!\d+|[-])))+(?:[-_. ]+(?<special>special|ova|ovd))?.*?(?<hash>\[\w{8}\])?(?:$|\.mkv)", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeSubGroupTitleAbsoluteEpisodeNumberExp();
    
    [GeneratedRegex(@"^\[(?<subgroup>.+?)\][-_. ]?(?<title>.+?)[-_. ]+\(?(?:[-_. ]?#?(?<absoluteepisode>\d{2,3}(\.\d{1,2})?(?!\d+)))+\)?(?:[-_. ]+(?<special>special|ova|ovd))?.*?(?<hash>\[\w{8}\])?(?:$|\.mkv)", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeSubGroupTitleAbsoluteEpisodeNumberSpecialExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:(?:[-_\W](?<![()[!]))+S?(?<season>(?<!\d+)(?:\d{1,2}|\d{4})(?!\d+))(?:(?:[ex]|[-_. ]e){1,2}(?<episode>\d{1,3}(?!\d+)))+){2,}", RegexOptions.IgnoreCase)]
    private static partial Regex MultiEpisodeRepeatedExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:(?:[-_\W](?<![()[!]))+S?(?<season>(?<!\d+)(?:\d{1,2})(?!\d+))(?:[ex]|\W[ex]){1,2}(?<episode>(?!265|264)\d{2,3}(?!\d+|(?:[ex]|\W[ex]|_|-){1,2})))", RegexOptions.IgnoreCase)]
    private static partial Regex SingleEpisodesWithTitleExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:[-_\W](?<![()[!]))+(?:S?(?<season>(?<!\d+)\d{1,2}(?!\d+))(?:(?:[ex]|\W[ex]){1,2}(?<episode>(?<!\d+)\d{2}(?!\d+)))).+?(?:[-_. ]?(?<absoluteepisode>(?<!\d+)\d{3}(\.\d{1,2})?(?!\d+)))+.+?\[(?<subgroup>.+?)\](?:$|\.mkv)", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeTitleSeasonEpisodeNumberAbsoluteEpisodeNumberSubGroupExp();

    [GeneratedRegex(@"^(?<title>.+?)[-_. ]Episode(?:[-_. ]+(?<absoluteepisode>\d{2,3}(\.\d{1,2})?(?!\d+)))+(?:.+?)\[(?<subgroup>.+?)\].*?(?<hash>\[\w{8}\])?(?:$|\.)", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeTitleEpisodeAbsoluteEpisodeNumberSubGroupHashExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:(?:_|-|\s|\.)+(?<absoluteepisode>\d{3}(\.\d{1,2})(?!\d+)))+(?:.+?)\[(?<subgroup>.+?)\].*?(?<hash>\[\w{8}\])?(?:$|\.)", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeTitleAbsoluteEpisodeNumberSubGroupHashExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:(?:_|-|\s|\.)+(?<absoluteepisode>\d{2,3}(\.\d{1,2})?(?!\d+)))+(?:[-_. ]+(?<special>special|ova|ovd))?[-_. ]+.*?(?<hash>\[\w{8}\])(?:$|\.)", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeTitleAbsoluteEpisodeNumberHashExp();

    [GeneratedRegex(@"^(?<title>.+?)?\W*(?<airdate>\d{4}\W+[0-1][0-9]\W+[0-3][0-9])(?!\W+[0-3][0-9])[-_. ](?:s?(?<season>(?<!\d+)(?:\d{1,2})(?!\d+)))(?:[ex](?<episode>(?<!\d+)(?:\d{1,3})(?!\d+)))/i", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodesWithAirdateAndSeasonEpisodeNumberCaptureSeasonEpisodeOnlyExp();

    [GeneratedRegex(@"^(?<title>.+?)?\W*(?<airyear>\d{4})\W+(?<airmonth>[0-1][0-9])\W+(?<airday>[0-3][0-9])(?!\W+[0-3][0-9]).+?(?:s?(?<season>(?<!\d+)(?:\d{1,2})(?!\d+)))(?:[ex](?<episode>(?<!\d+)(?:\d{1,3})(?!\d+)))/i", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodesWithAirdateAndSeasonEpisodeNumberExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:(?:[-_\W](?<![()[!]))+S(?<season>(?<!\d+)(?:\d{1,2})(?!\d+))(?:e|\We|_){1,2}(?<episode>\d{2,3}(?!\d+))(?:(?:-|e|\We|_){1,2}(?<episode1>\d{2,3}(?!\d+)))*)\W?(?!\\)", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodesWithTitleSingleEpisodesMultiEpisodeExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:(?:[-_\W](?<![()[!]))+S(?<season>(?<!\d+)(?:\d{4})(?!\d+))(?:e|\We|_){1,2}(?<episode>\d{2,3}(?!\d+))(?:(?:-|e|\We|_){1,2}(?<episode1>\d{2,3}(?!\d+)))*)\W?(?!\\)", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodesWithTitle4DigitSeasonNumberSingleEpisodesMultiEpisodeExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:(?:[-_\W](?<![()[!]))+(?<season>(?<!\d+)(?:\d{4})(?!\d+))(?:x|\Wx){1,2}(?<episode>\d{2,3}(?!\d+))(?:(?:-|x|\Wx|_){1,2}(?<episode1>\d{2,3}(?!\d+)))*)\W?(?!\\)", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodesWithTitle4DigitSeasonNumberSingleEpisodesMultiEpisodeExp2();

    [GeneratedRegex(@"^(?<title>.+?)[-_. ]+S(?<season>(?<!\d+)(?:\d{1,2})(?!\d+))\W?-\W?S?(?<season1>(?<!\d+)(?:\d{1,2})(?!\d+))", RegexOptions.IgnoreCase)]
    private static partial Regex MultiSeasonPackExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:\W+S(?<season>(?<!\d+)(?:\d{1,2})(?!\d+))\W+(?:(?:Part\W?|(?<!\d+\W+)e)(?<seasonpart>\d{1,2}(?!\d+)))+)", RegexOptions.IgnoreCase)]
    private static partial Regex PartialSeasonPackExp();

    [GeneratedRegex(@"^(?<title>.+?\d{4})(?:\W+(?:(?:Part\W?|e)(?<episode>\d{1,2}(?!\d+)))+)", RegexOptions.IgnoreCase)]
    private static partial Regex MiniSeriesWithYearInTitleExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:[-._ ][e])(?<episode>\d{2,3}(?!\d+))(?:(?:-?[e])(?<episode1>\d{2,3}(?!\d+)))+", RegexOptions.IgnoreCase)]
    private static partial Regex MiniSeriesMultiEpisodesExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:\W+(?:(?:Part\W?|(?<!\d+\W+)e)(?<episode>\d{1,2}(?!\d+)))+)", RegexOptions.IgnoreCase)]
    private static partial Regex MiniSeriesEpisodesExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:\W+(?:Part[-._ ](?<episode>One|Two|Three|Four|Five|Six|Seven|Eight|Nine)(>[-._ ])))", RegexOptions.IgnoreCase)]
    private static partial Regex MiniSeriesEpisodesExp2();

    [GeneratedRegex(@"^(?<title>.+?)(?:\W+(?:(?<episode>(?<!\d+)\d{1,2}(?!\d+))of\d+)+)", RegexOptions.IgnoreCase)]
    private static partial Regex MiniSeriesEpisodesExp3();

    [GeneratedRegex(@"(?:.*(?:""|^))(?<title>.*?)(?:[-_\W](?<![()[]))+(?:\W?Season\W?)(?<season>(?<!\d+)\d{1,2}(?!\d+))(?:\W|_)+(?:Episode\W)(?:[-_. ]?(?<episode>(?<!\d+)\d{1,2}(?!\d+)))+", RegexOptions.IgnoreCase)]
    private static partial Regex SupportsSeason01Episode03Exp();
    
    [GeneratedRegex(@"(?:.*(?:^))(?<title>.*?)[-._ ]+\[S(?<season>(?<!\d+)\d{2}(?!\d+))(?:[E-]{1,2}(?<episode>(?<!\d+)\d{2}(?!\d+)))+\]", RegexOptions.IgnoreCase)]
    private static partial Regex MultiEpisodeWithEpisodesInSquareBracketsExp();

    [GeneratedRegex(@"(?:.*(?:^))(?<title>.*?)S(?<season>(?<!\d+)\d{2}(?!\d+))(?:E(?<episode>(?<!\d+)\d{2}(?!\d+)))+", RegexOptions.IgnoreCase)]
    private static partial Regex MultiEpisodeReleaseWithNoSpaceBetweenSeriesTitleAndSeasonExp();

    [GeneratedRegex(@"(?:.*(?:""|^))(?<title>.*?)(?:\W?|_)S(?<season>(?<!\d+)\d{1,2}(?!\d+))(?:\W|_)?Ep?[ ._]?(?<episode>(?<!\d+)\d{1,2}(?!\d+))", RegexOptions.IgnoreCase)]
    private static partial Regex SingleEpisodeSeasonOrEpisodeExp();

    [GeneratedRegex(@"(?:.*(?:""|^))(?<title>.*?)(?:\W?|_)S(?<season>(?<!\d+)\d{3}(?!\d+))(?:\W|_)?E(?<episode>(?<!\d+)\d{1,2}(?!\d+))", RegexOptions.IgnoreCase)]
    private static partial Regex ThreeDigitSeasonExp();

    [GeneratedRegex(@"^(?:(?<title>.+?)(?:_|-|\s|\.)+)(?:S?(?<season>(?<!\d+)\d{1,2}(?!\d+)))(?:(?:-|[ex]|\W[ex]|_){1,2}(?<episode>(?<!\d+)\d{5}(?!\d+)))", RegexOptions.IgnoreCase)]
    private static partial Regex FiveDigitEpisodeNumberWithTitleExp();

    [GeneratedRegex(@"^(?:(?<title>.+?)(?:_|-|\s|\.)+)(?:S?(?<season>(?<!\d+)\d{1,2}(?!\d+)))(?:(?:[-_. ]{1,3}ep){1,2}(?<episode>(?<!\d+)\d{5}(?!\d+)))", RegexOptions.IgnoreCase)]
    private static partial Regex FiveDigitMultiEpisodeWithTitleExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:_|-|\s|\.)+S(?<season>\d{2}(?!\d+))(\W-\W)E(?<episode>(?<!\d+)\d{2}(?!\d+))(?!\\)", RegexOptions.IgnoreCase)]
    private static partial Regex SeparatedSeasonAndEpisodeNumbersExp();

    [GeneratedRegex(@"^(?<title>.+?S\d{1,2})[-_. ]{3,}(?:EP)?(?<absoluteepisode>\d{2,3}(\.\d{1,2})?(?!\d+))", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeTitleWithSeasonNumberAbsoluteEpisodeNumberExp();

    [GeneratedRegex(@"^(?:\[(?<subgroup>.+?)\][-_. ]?)?(?<title>.+?)[-_. ]+?(?:Episode[-_. ]+?)(?<absoluteepisode>\d{1}(\.\d{1,2})?(?!\d+))", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeFrenchTitlesWithSingleEpisodeNumbersExp();

    [GeneratedRegex(@"^(?<title>.+?)\W(?:S|Season)\W?(?<season>\d{1,2}(?!\d+))(\W+|_|$)(?<extras>EXTRAS|SUBPACK)?(?!\\)", RegexOptions.IgnoreCase)]
    private static partial Regex SeasonOnlyReleasesExp();

    [GeneratedRegex(@"^(?<title>.+?)\W(?:S|Season)\W?(?<season>\d{4}(?!\d+))(\W+|_|$)(?<extras>EXTRAS|SUBPACK)?(?!\\)", RegexOptions.IgnoreCase)]
    private static partial Regex FourDigitSeasonOnlyReleasesExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:(?:[-_\W](?<![()[!]))+\[S?(?<season>(?<!\d+)\d{1,2}(?!\d+))(?:(?:-|[ex]|\W[ex]|_){1,2}(?<episode>(?<!\d+)\d{2}(?!\d+|i|p)))+\])\W?(?!\\)", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodesWithTitleAndSeasonEpisodeInSquareBracketsExp();

    [GeneratedRegex(@"^(?<title>.+?)?(?:(?:[_.](?<![()[!]))+(?<season>(?<!\d+)[1-9])(?<episode>[1-9][0-9]|[0][1-9])(?![a-z]|\d+))+(?:[_.]|$)", RegexOptions.IgnoreCase)]
    private static partial Regex Supports103_113NamingExp();

    [GeneratedRegex(@"^(?:S?(?<season>(?<!\d+)\d{1,2}(?!\d+))(?:(?:-|[ex]|\W[ex]|_){1,2}(?<episode>\d{4}(?!\d+|i|p)))+)(\W+|_|$)(?!\\)", RegexOptions.IgnoreCase)]
    private static partial Regex FourDigitEpisodeNumberEpisodesWithoutTitleSingleAndMultiExp();

    [GeneratedRegex(@"^(?<title>.+?)(?:(?:[-_\W](?<![()[!]))+S?(?<season>(?<!\d+)\d{1,2}(?!\d+))(?:(?:-|[ex]|\W[ex]|_){1,2}(?<episode>\d{4}(?!\d+|i|p)))+)\W?(?!\\)", RegexOptions.IgnoreCase)]
    private static partial Regex FourDigitEpisodeNumberEpisodesWithTitleSingleAndMultiExp();

    [GeneratedRegex(@"^(?<title>.+?)?\W*(?<airyear>\d{4})[-_. ]+(?<airmonth>[0-1][0-9])[-_. ]+(?<airday>[0-3][0-9])(?![-_. ]+[0-3][0-9])", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodesWithAirdateExp();

    [GeneratedRegex(@"^(?<title>.+?)?\W*(?<airmonth>[0-1][0-9])[-_. ]+(?<airday>[0-3][0-9])[-_. ]+(?<airyear>\d{4})(?!\d+)", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodesWithAirdateExp2();
    
    [GeneratedRegex(@"^(?<title>.+?)?(?:(?:[-_\W](?<![()[!]))*(?<season>(?<!\d+|\(|\[|e|x)\d{2})(?<episode>(?<!e|x)\d{2}(?!p|i|\d+|\)|\]|\W\d+|\W(?:e|ep|x)\d+)))+(\W+|_|$)(?!\\)", RegexOptions.IgnoreCase)]
    private static partial Regex Supports1103_1113NamingExp();

    [GeneratedRegex(@"^(?<title>.*?)(?:(?:[-_\W](?<![()[!]))+S?(?<season>(?<!\d+)\d{1,2}(?!\d+))(?:(?:-|[ex]){1,2}(?<episode>\d{1}))+)+(\W+|_|$)(?!\\)", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodesWithSingleDigitEpisodeNumberExp();

    [GeneratedRegex(@"^(?:Season(?:_|-|\s|\.)(?<season>(?<!\d+)\d{1,2}(?!\d+)))(?:_|-|\s|\.)(?<episode>(?<!\d+)\d{1,2})", RegexOptions.IgnoreCase)]
    private static partial Regex ITunesSeason1_05TitleQualityExp();

    [GeneratedRegex(@"^(?:(?<season>(?<!\d+)(?:\d{1,2})(?!\d+))(?:-(?<episode>\d{2,3}(?!\d+))))", RegexOptions.IgnoreCase)]
    private static partial Regex ITunes1_05TitleQualityExp();

    [GeneratedRegex(@"^(?:\[(?<subgroup>.+?)\][-_. ]?)?(?<title>.+?)(?:_|\s|\.)+(?:e|ep)(?<absoluteepisode>\d{2,3}(\.\d{1,2})?)-(?<absoluteepisode1>(?<!\d+)\d{1,2}(\.\d{1,2})?(?!\d+|-)).*?(?<hash>\[\w{8}\])?(?:$|\.)", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeRange_TitleAbsoluteEpisodeNumberExp();

    [GeneratedRegex(@"^(?:\[(?<subgroup>.+?)\][-_. ]?)?(?<title>.+?)(?:(?:_|-|\s|\.)+(?:e|ep)(?<absoluteepisode>\d{2,4}(\.\d{1,2})?))+.*?(?<hash>\[\w{8}\])?(?:$|\.)", RegexOptions.IgnoreCase)]
    private static partial Regex Anime_TitleAbsoluteEpisodeNumberExp();

    [GeneratedRegex(@"^(?<title>.+?)[-_. ](?:Episode)(?:[-_. ]+(?<absoluteepisode>(?<!\d+)\d{2,3}(\.\d{1,2})?(?!\d+)))+(?:_|-|\s|\.)*?(?<hash>\[.{8}\])?(?:$|\.)?", RegexOptions.IgnoreCase)]
    private static partial Regex Anime_TitleEpisodeAbsoluteEpisodeNumberExp();

    [GeneratedRegex(@"^(?:\[(?<subgroup>.+?)\][-_. ]?)?(?<title>.+?)[_. ]+(?<absoluteepisode>(?<!\d+)\d{1,2}(\.\d{1,2})?(?!\d+))-(?<absoluteepisode1>(?<!\d+)\d{1,2}(\.\d{1,2})?(?!\d+|-))(?:_|\s|\.)*?(?<hash>\[.{8}\])?(?:$|\.)?", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeRange_TitleAbsoluteEpisodeNumberExp2();

    [GeneratedRegex(@"^(?:\[(?<subgroup>.+?)\][-_. ]?)?(?<title>.+?)(?:[-_. ]+(?<absoluteepisode>(?<!\d+)\d{2,3}(\.\d{1,2})?(?!\d+)))+(?:_|-|\s|\.)*?(?<hash>\[.{8}\])?(?:$|\.)?", RegexOptions.IgnoreCase)]
    private static partial Regex Anime_TitleAbsoluteEpisodeNumberExp2();

    [GeneratedRegex(@"^(?:\[(?<subgroup>.+?)\][-_. ]?)?(?<title>.+?)(?:(?:[-_\W](?<![()[!]))+(?<absoluteepisode>(?<!\d+)\d{2,3}(\.\d{1,2})?(?!\d+)))+(?:_|-|\s|\.)*?(?<hash>\[.{8}\])?(?:$|\.)?", RegexOptions.IgnoreCase)]
    private static partial Regex Anime_TitleAbsoluteEpisodeNumberExp3();

    [GeneratedRegex(@"^(?<title>.+?)[-_. ](?<season>[0]?\d?)(?:(?<episode>\d{2}){2}(?!\d+))[-_. ]", RegexOptions.IgnoreCase)]
    private static partial Regex ExtantTerribleMultiEpisodeNamingExp();
    
    
    private static List<Func<Regex>> _validRegexes =
    [
        DailyEpisodesWithoutTitleExp,
        MultiPartEpisodesWithoutTitleExp,
        MultiEpisodeWithSingleEpisodeNumbersExp,
        MultiEpisodeWithTitleAndTrailingInfoInSlashesExp,
        EpisodesWithoutTitleMultiExp,
        EpisodesWithoutTitleSingleExp,
        AnimeSubGroupTitleEpisodeAbsoluteEpisodeNumberExp,
        AnimeSubGroupTitleAbsoluteEpisodeNumberSeasonEpisodeExp,
        AnimeSubGroupTitleSeasonEpisodeAbsoluteEpisodeNumberExp,
        AnimeSubGroupTitleSeasonEpisodeExp,
        AnimeSubGroupTitleWithTrailingNumberAbsoluteEpisodeNumberExp,
        AnimeSubGroupTitleAbsoluteEpisodeNumberExp,
        AnimeSubGroupTitleAbsoluteEpisodeNumberSpecialExp,
        MultiEpisodeRepeatedExp,
        SingleEpisodesWithTitleExp,
        AnimeTitleSeasonEpisodeNumberAbsoluteEpisodeNumberSubGroupExp,
        AnimeTitleEpisodeAbsoluteEpisodeNumberSubGroupHashExp,
        AnimeTitleAbsoluteEpisodeNumberSubGroupHashExp,
        AnimeTitleAbsoluteEpisodeNumberHashExp,
        EpisodesWithAirdateAndSeasonEpisodeNumberCaptureSeasonEpisodeOnlyExp,
        EpisodesWithAirdateAndSeasonEpisodeNumberExp,
        EpisodesWithTitleSingleEpisodesMultiEpisodeExp,
        EpisodesWithTitle4DigitSeasonNumberSingleEpisodesMultiEpisodeExp,
        EpisodesWithTitle4DigitSeasonNumberSingleEpisodesMultiEpisodeExp2,
        MultiSeasonPackExp,
        PartialSeasonPackExp,
        MiniSeriesWithYearInTitleExp,
        MiniSeriesMultiEpisodesExp,
        MiniSeriesEpisodesExp,
        MiniSeriesEpisodesExp2,
        MiniSeriesEpisodesExp3,
        SupportsSeason01Episode03Exp,
        MultiEpisodeWithEpisodesInSquareBracketsExp,
        MultiEpisodeReleaseWithNoSpaceBetweenSeriesTitleAndSeasonExp,
        SingleEpisodeSeasonOrEpisodeExp,
        ThreeDigitSeasonExp,
        FiveDigitEpisodeNumberWithTitleExp,
        SeparatedSeasonAndEpisodeNumbersExp,
        AnimeTitleWithSeasonNumberAbsoluteEpisodeNumberExp,
        AnimeFrenchTitlesWithSingleEpisodeNumbersExp,
        SeasonOnlyReleasesExp,
        FourDigitSeasonOnlyReleasesExp,
        EpisodesWithTitleAndSeasonEpisodeInSquareBracketsExp,
        Supports103_113NamingExp,
        FourDigitEpisodeNumberEpisodesWithoutTitleSingleAndMultiExp,
        FourDigitEpisodeNumberEpisodesWithTitleSingleAndMultiExp,
        EpisodesWithAirdateExp,
        EpisodesWithAirdateExp2,
        Supports1103_1113NamingExp,
        EpisodesWithSingleDigitEpisodeNumberExp,
        ITunesSeason1_05TitleQualityExp,
        ITunes1_05TitleQualityExp,
        AnimeRange_TitleAbsoluteEpisodeNumberExp,
        Anime_TitleAbsoluteEpisodeNumberExp,
        Anime_TitleEpisodeAbsoluteEpisodeNumberExp,
        AnimeRange_TitleAbsoluteEpisodeNumberExp2,
        Anime_TitleAbsoluteEpisodeNumberExp2,
        Anime_TitleAbsoluteEpisodeNumberExp3,
        ExtantTerribleMultiEpisodeNamingExp,
    ];
}