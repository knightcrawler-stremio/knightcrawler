namespace Producer.Features.ParseTorrentTitle;

public static partial class SeasonParser
{
    [GeneratedRegex(@"^[0-9a-zA-Z]{32}", RegexOptions.IgnoreCase)]
    private static partial Regex GenericMatchForMd5AndMixedCaseHashesExp();

    [GeneratedRegex(@"^[a-z0-9]{24}$", RegexOptions.IgnoreCase)]
    private static partial Regex GenericMatchForShorterLowerCaseHashesExp();

    [GeneratedRegex(@"^[A-Z]{11}\d{3}$", RegexOptions.IgnoreCase)]
    private static partial Regex FormatSeenOnSomeNZBGeekReleasesExp();

    [GeneratedRegex(@"^[a-z]{12}\d{3}$", RegexOptions.IgnoreCase)]
    private static partial Regex FormatSeenOnSomeNZBGeekReleasesExp2();

    [GeneratedRegex(@"^Backup_\d{5,}S\d{2}-\d{2}$", RegexOptions.IgnoreCase)]
    private static partial Regex BackupFilenameExp();

    [GeneratedRegex(@"^123$", RegexOptions.IgnoreCase)]
    private static partial Regex StartedAppearingDecember2014Exp();

    [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase)]
    private static partial Regex StartedAppearingJanuary2015Exp();

    [GeneratedRegex(@"^b00bs$", RegexOptions.IgnoreCase)]
    private static partial Regex StartedAppearingJanuary2015Exp2();

    [GeneratedRegex(@"^\d{6}_\d{2}$", RegexOptions.IgnoreCase)]
    private static partial Regex StartedAppearingAugust2018Exp();
    
    private static List<Func<Regex>> _rejectedRegex =
    [
        GenericMatchForMd5AndMixedCaseHashesExp,
        GenericMatchForShorterLowerCaseHashesExp,
        FormatSeenOnSomeNZBGeekReleasesExp,
        FormatSeenOnSomeNZBGeekReleasesExp2,
        BackupFilenameExp,
        StartedAppearingDecember2014Exp,
        StartedAppearingJanuary2015Exp,
        StartedAppearingJanuary2015Exp2,
        StartedAppearingAugust2018Exp
    ];
}