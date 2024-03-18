namespace Producer.Features.PTN;

public static partial class ParserRegex
{
    private const string NonEnglishChars = @"[\u3040-\u30ff\u3400-\u4dbf\u4e00-\u9fff\uf900-\ufaff\uff66-\uff9f\u0400-\u04ff]";
    [GeneratedRegex(NonEnglishChars)]
    public static partial Regex NonEnglishCharsRegex();

    [GeneratedRegex(@"[^/|(]*" + NonEnglishChars + @"[^/|]*[/|]|[/|][^/|(]*" + NonEnglishChars + @"[^/|]*", RegexOptions.Compiled)]
    public static partial Regex AltTitlesRegex();

    [GeneratedRegex(@"(?<=[a-zA-Z][^" + NonEnglishChars + @"]+)" + NonEnglishChars + @".*" + NonEnglishChars + @"|" + NonEnglishChars + @".*" + NonEnglishChars + @"(?=[^" + NonEnglishChars + @"]+[a-zA-Z])", RegexOptions.Compiled)]
    public static partial Regex NotOnlyNonEnglishRegex();

    [GeneratedRegex(@"^[^\w" + NonEnglishChars + @"#[【★]+|[ \-:/\\[|{(#$&^]+$", RegexOptions.Compiled)]
    private static partial Regex NotAllowedSymbolsAtStartAndEnd();

    [GeneratedRegex(@"^[^\w" + NonEnglishChars + @"#]+|]$", RegexOptions.Compiled)]
    public static partial Regex RemainingNotAllowedSymbolsAtStartAndEnd();

    [GeneratedRegex(@"\([^)]*[\u0400-\u04ff][^)]*\)$|(?<=\/.*)\(.*\)$", RegexOptions.Compiled)]
    public static partial Regex RussianCastRegex();

    [GeneratedRegex(@"^\[([^[\]]+)]")]
    public static partial Regex BeforeTitle();

    [GeneratedRegex(@"^[[【★].*[\]】★][ .]?(.+)", RegexOptions.IgnoreCase, "en-GB")]
    public static partial Regex TitleMatchOne();

    [GeneratedRegex(@"(.+)[ .]?[[【★].*[\]】★]$", RegexOptions.IgnoreCase, "en-GB")]
    public static partial Regex TitleMatchTwo();
    
    [GeneratedRegex(@"(season|episode)s?.?\d?", RegexOptions.IgnoreCase, "en-GB")]
    public static partial Regex SeasonEpisode();
    [GeneratedRegex(@"[se]\d\d", RegexOptions.IgnoreCase, "en-GB")]
    public static partial Regex SeasonShort();
    [GeneratedRegex(@"\b(tv|complete)\b", RegexOptions.IgnoreCase, "en-GB")]
    public static partial Regex TvOrComplete();
    [GeneratedRegex(@"\b(saison|stage).?\d", RegexOptions.IgnoreCase, "en-GB")]
    public static partial Regex SeasonStage();
    [GeneratedRegex(@"[a-z]\s?\-\s?\d{2,4}\b", RegexOptions.IgnoreCase, "en-GB")]
    public static partial Regex Season();
    [GeneratedRegex(@"\d{2,4}\s?\-\s?\d{2,4}\b", RegexOptions.IgnoreCase, "en-GB")]
    public static partial Regex SeasonTwo();

    [GeneratedRegex(@"\D+")]
    public static partial Regex Numbers();

    [GeneratedRegex(@"\W+")]
    public static partial Regex Word();

    [GeneratedRegex(@"\D+")]
    public static partial Regex YearDigits();
}