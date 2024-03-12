namespace Tissue.Extensions;

public static partial class StringExtensions
{
    [GeneratedRegex("[^a-zA-Z0-9 ]")]
    private static partial Regex NotAlphaNumeric();

    public static bool IsNullOrEmpty(this string? value) =>
        string.IsNullOrEmpty(value);

    private static readonly char[] separator = [' '];

    public static string NormalizeTitle(this string title)
    {
        var alphanumericTitle = NotAlphaNumeric().Replace(title, " ");

        var words = alphanumericTitle.Split(separator, StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.ToLower());

        var normalizedTitle = string.Join(" ", words);

        return normalizedTitle;
    }
}
