using System.Text.RegularExpressions;

namespace SharedContracts.Extensions;

public static partial class StringExtensions
{
    [GeneratedRegex("[^a-zA-Z0-9 ]")]
    private static partial Regex NotAlphaNumeric();

    [GeneratedRegex(@"\s*\([^)]*\)|\s*\b\d{4}\b")]
    private static partial Regex CleanTitleForImdb();

    private static readonly char[] separator = [' '];

    public static bool IsNullOrEmpty(this string? value) =>
        string.IsNullOrEmpty(value);

    public static string NormalizeTitle(this string title)
    {
        var alphanumericTitle = NotAlphaNumeric().Replace(title, " ");

        var words = alphanumericTitle.Split(separator, StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.ToLower());

        var normalizedTitle = string.Join(" ", words);

        return normalizedTitle;
    }

    public static string RemoveMatches(this string input, IEnumerable<Func<Regex>> regexPatterns) =>
        regexPatterns.Aggregate(input, (current, regex) => regex().Replace(current, string.Empty));

    public static string CleanTorrentTitleForImdb(this string title)
    {
        var cleanTitle = CleanTitleForImdb().Replace(title, "").Trim();

        cleanTitle = cleanTitle.ToLower();

        return cleanTitle;
    }
}
