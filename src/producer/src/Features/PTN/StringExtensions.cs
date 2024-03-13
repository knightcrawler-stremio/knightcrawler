namespace Producer.Features.PTN;

public static class StringExtensions
{
    public static string CleanTitle(this string rawTitle)
    {
        var cleanedTitle = rawTitle;

        if (!cleanedTitle.Contains(' ', StringComparison.OrdinalIgnoreCase) && cleanedTitle.Contains('.', StringComparison.OrdinalIgnoreCase))
        {
            cleanedTitle = cleanedTitle.Replace(".", " ", StringComparison.OrdinalIgnoreCase);
        }

        cleanedTitle = cleanedTitle
            .Replace("_", " ", StringComparison.OrdinalIgnoreCase)
            .Replace("(movie)", "", StringComparison.OrdinalIgnoreCase)
            .Replace("[movie]", "", StringComparison.OrdinalIgnoreCase);

        cleanedTitle = ParserRegex.NonEnglishCharsRegex().Replace(cleanedTitle, "");
        cleanedTitle = ParserRegex.RussianCastRegex().Replace(cleanedTitle, "");
        cleanedTitle = ParserRegex.TitleMatchOne().Replace(cleanedTitle, "$1");
        cleanedTitle = ParserRegex.TitleMatchTwo().Replace(cleanedTitle, "$1");
        cleanedTitle = ParserRegex.AltTitlesRegex().Replace(cleanedTitle, "");
        cleanedTitle = ParserRegex.NotOnlyNonEnglishRegex().Replace(cleanedTitle, "");
        cleanedTitle = ParserRegex.RemainingNotAllowedSymbolsAtStartAndEnd().Replace(cleanedTitle, "");

        cleanedTitle = cleanedTitle.Trim();


        return cleanedTitle;
    }
}
