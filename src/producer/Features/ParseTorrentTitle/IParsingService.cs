namespace Producer.Features.ParseTorrentTitle;

public interface IParsingService
{
    ParsedFilename Parse(string name);
    string Naked(string title);
    List<string> GrabYears(string str);
    List<int> GrabPossibleSeasonNums(string str);
    bool HasYear(string test, List<string> years, bool strictCheck = false);
    string RemoveDiacritics(string str);
    string RemoveRepeats(string str);
    int RomanToDecimal(string roman);
    string ReplaceRomanWithDecimal(string input);
    bool StrictEqual(string title1, string title2);
    int CountTestTermsInTarget(string test, string target, bool shouldBeInSequence = false);
    bool FlexEq(string test, string target, List<string> years);
    bool MatchesTitle(string target, List<string> years, string test);
    bool IncludesMustHaveTerms(List<string> mustHaveTerms, string testTitle);
    bool HasNoBannedTerms(string targetTitle, string testTitle);
    bool HasNoBannedTerms(string targetTitle);
    bool MeetsTitleConditions(string targetTitle, List<string> years, string testTitle);
    int CountUncommonWords(string title);
}
