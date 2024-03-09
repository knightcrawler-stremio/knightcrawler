namespace Producer.Features.Crawlers.Dmm;

public partial class ParsingService
{
    [GeneratedRegex(@"[^a-z0-9]")]
    private static partial Regex NakedMatcher();

    [GeneratedRegex(@"\d{4}")]
    private static partial Regex GrabYearsMatcher();

    [GeneratedRegex(@"\d+")]
    private static partial Regex GrabPossibleSeasonNumsMatcher();

    [GeneratedRegex(@"(.)\1+")]
    private static partial Regex RemoveRepeatsMatcher();

    [GeneratedRegex(@"m{0,4}(cm|cd|d?c{0,3})(xc|xl|l?x{0,3})(ix|iv|v?i{0,3})")]
    private static partial Regex ReplaceRomanWithDecimalMatcher();
    
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceMatcher();
    
    [GeneratedRegex(@"\W+")]
    private static partial Regex WordMatcher();
    
    
    [GeneratedRegex(@"'s|\s&\s|\W")]
    private static partial Regex WordProcessingMatcher();
}