namespace Producer.Features.ParseTorrentTitle;

public partial class ParsingService : IParsingService
{
    private readonly IWordCollections _wordCollections;
    private readonly ITorrentTitleParser _torrentTitleParser;
    private static readonly char[] WhitespaceSeparator = [' '];
    private HashSet<string> _compoundWords = [];

    public ParsingService(IWordCollections wordCollections, ITorrentTitleParser torrentTitleParser)
    {
        _wordCollections = wordCollections;
        _torrentTitleParser = torrentTitleParser;

        _compoundWords.UnionWith(_wordCollections.AdultCompoundPhrases);
        _compoundWords.UnionWith(_wordCollections.Jav);
        _compoundWords.UnionWith(_wordCollections.AdultStars);
    }

    public string Naked(string title) =>
        NakedMatcher().Replace(title.ToLower(), "");

    public List<string> GrabYears(string str)
    {
        var matches = GrabYearsMatcher().Matches(str);
        return matches
            .Select(m => m.Value)
            .Where(n => int.Parse(n) > 1900 && int.Parse(n) <= DateTime.Now.Year)
            .ToList();
    }

    public List<int> GrabPossibleSeasonNums(string str)
    {
        var matches = GrabPossibleSeasonNumsMatcher().Matches(str);
        return matches
            .Select(m => int.Parse(m.Value))
            .Where(n => n is > 0 and <= 500)
            .ToList();
    }

    public bool HasYear(string test, List<string> years, bool strictCheck = false) =>
        strictCheck
            ? years.Any(test.Contains)
            : years.Any(year =>
            {
                var intYear = int.Parse(year);
                return test.Contains(year) ||
                       test.Contains($"{intYear + 1}") ||
                       test.Contains($"{intYear - 1}");
            });

    public string RemoveDiacritics(string str)
    {
        var normalizedString = str.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public string RemoveRepeats(string str) => RemoveRepeatsMatcher().Replace(str, "$1");

    public int RomanToDecimal(string roman)
    {
        var romanNumerals = new Dictionary<char, int>
        {
            {'I', 1},
            {'V', 5},
            {'X', 10},
            {'L', 50},
            {'C', 100},
            {'D', 500},
            {'M', 1000}
        };

        var total = 0;
        var prevValue = 0;

        for (var i = roman.Length - 1; i >= 0; i--)
        {
            var currentValue = romanNumerals[roman[i].ToString().ToUpper()[0]];
            total = currentValue < prevValue ? total - currentValue : total + currentValue;
            prevValue = currentValue;
        }

        return total;
    }

    public string ReplaceRomanWithDecimal(string input) => ReplaceRomanWithDecimalMatcher().Replace(input, match => RomanToDecimal(match.Value).ToString());

    public bool StrictEqual(string title1, string title2)
    {
        title1 = WhitespaceMatcher().Replace(title1, "");
        title2 = WhitespaceMatcher().Replace(title2, "");

        return (title1.Length > 0 && title1 == title2) ||
               (Naked(title1).Length > 0 && Naked(title1) == Naked(title2)) ||
               (RemoveRepeats(title1).Length > 0 && RemoveRepeats(title1) == RemoveRepeats(title2)) ||
               (RemoveDiacritics(title1).Length > 0 && RemoveDiacritics(title1) == RemoveDiacritics(title2));
    }

    public int CountTestTermsInTarget(string test, string target, bool shouldBeInSequence = false)
    {
        var replaceCount = 0;
        var prevReplaceCount = 0;
        var prevOffset = 0;
        var prevLength = 0;
        const int wordTolerance = 5;

        var wordsInTitle = WordMatcher().Split(target).Where(e => !string.IsNullOrEmpty(e)).ToList();
        const int magicLength = 3;
        var testStr = test;

        var inSequenceTerms = 1;
        var longestSequence = 0;

        MatchEvaluator replacer = match =>
        {
            if (shouldBeInSequence && prevLength > 0 && match.Index >= wordTolerance)
            {
                if (inSequenceTerms > longestSequence)
                {
                    longestSequence = inSequenceTerms;
                }

                inSequenceTerms = 0;
            }
            prevOffset = match.Index;
            prevLength = match.Length;
            replaceCount++;
            inSequenceTerms++;
            return match.Value;
        };

        Action<string, bool, bool> wrapReplace = (newTerm, first, last) =>
        {
            var prefix = first ? @"\b" : "";
            var suffix = last ? @"\b" : "";
            testStr = Regex.Replace(testStr[(prevOffset + prevLength)..], $"{prefix}{newTerm}{suffix}", replacer);
        };

        var actual = wordsInTitle.Where((term, idx) =>
        {
            var first = idx == 0;
            var last = idx == wordsInTitle.Count - 1;
            testStr = testStr[(prevOffset + prevLength)..];
            wrapReplace(term, first, last);
            if (replaceCount > prevReplaceCount)
            {
                prevReplaceCount = replaceCount;
                return true;
            }
            if (RemoveDiacritics(term).Length >= magicLength)
            {
                wrapReplace(RemoveDiacritics(term), first, last);
                if (replaceCount > prevReplaceCount)
                {
                    prevReplaceCount = replaceCount;
                    return true;
                }
            }
            if (RemoveRepeats(term).Length >= magicLength)
            {
                wrapReplace(RemoveRepeats(term), first, last);
                if (replaceCount > prevReplaceCount)
                {
                    prevReplaceCount = replaceCount;
                    return true;
                }
            }
            if (Naked(term).Length >= magicLength)
            {
                wrapReplace(Naked(term), first, last);
                if (replaceCount > prevReplaceCount)
                {
                    prevReplaceCount = replaceCount;
                    return true;
                }
            }

            if (ReplaceRomanWithDecimal(term) == term)
            {
                return false;
            }

            wrapReplace(ReplaceRomanWithDecimal(term), first, last);

            if (replaceCount <= prevReplaceCount)
            {
                return false;
            }

            prevReplaceCount = replaceCount;
            return true;
        }).ToList();

        if (shouldBeInSequence)
        {
            return inSequenceTerms > longestSequence ? inSequenceTerms : longestSequence;
        }
        return actual.Count;
    }

    public bool FlexEq(string test, string target, List<string> years)
    {
        var movieTitle = _torrentTitleParser.Parse(test).Movie.Title.ToLower();
        var tvTitle = _torrentTitleParser.Parse(test).Show.Title.ToLower();

        var target2 = WhitespaceMatcher().Replace(target, "");
        var test2 = WhitespaceMatcher().Replace(test, "");

        var magicLength = HasYear(test, years) ? 3 : 5;

        if (Naked(target2).Length >= magicLength && test2.Contains(Naked(target2)))
        {
            return true;
        }

        if (RemoveRepeats(target2).Length >= magicLength && test2.Contains(RemoveRepeats(target2)))
        {
            return true;
        }
        if (RemoveDiacritics(target2).Length >= magicLength && test2.Contains(RemoveDiacritics(target2)))
        {
            return true;
        }
        if (target2.Length >= Math.Ceiling(magicLength * 1.5) && test2.Contains(target2))
        {
            return true;
        }

        return StrictEqual(target, movieTitle) || StrictEqual(target, tvTitle);
    }

    public bool MatchesTitle(string target, List<string> years, string test)
    {
        target = target.ToLower();
        test = test.ToLower();

        var splits = WordMatcher().Split(target).Where(e => !string.IsNullOrEmpty(e)).ToList();
        var containsYear = HasYear(test, years);

        if (FlexEq(test, target, years))
        {
            var sequenceCheck = CountTestTermsInTarget(test, string.Join(' ', splits), true);
            return containsYear || sequenceCheck >= 0;
        }

        var totalTerms = splits.Count;
        if (totalTerms == 0 || (totalTerms <= 2 && !containsYear))
        {
            return false;
        }

        var keyTerms = splits.Where(s => (s.Length > 1 && !_wordCollections.CommonWords.Contains(s)) || s.Length > 5).ToList();
        keyTerms.AddRange(target.Split(WhitespaceSeparator, StringSplitOptions.RemoveEmptyEntries).Where(e => e.Length > 2));
        var keySet = new HashSet<string>(keyTerms);
        var commonTerms = splits.Where(s => !keySet.Contains(s)).ToList();

        var hasYearScore = totalTerms * 1.5;
        var totalScore = keyTerms.Count * 2 + commonTerms.Count + hasYearScore;

        if (keyTerms.Count == 0 && totalTerms <= 2 && !containsYear)
        {
            return false;
        }

        var foundKeyTerms = CountTestTermsInTarget(test, string.Join(' ', keyTerms));
        var foundCommonTerms = CountTestTermsInTarget(test, string.Join(' ', commonTerms));
        var score = foundKeyTerms * 2 + foundCommonTerms + (containsYear ? hasYearScore : 0);

        return Math.Floor(score / 0.85) >= totalScore;
    }

    public bool IncludesMustHaveTerms(List<string> mustHaveTerms, string testTitle) =>
        mustHaveTerms.All(term =>
        {
            var newTitle = testTitle.Replace(term, "");
            if (newTitle != testTitle)
            {
                testTitle = newTitle;
                return true;
            }

            newTitle = testTitle.Replace(RemoveDiacritics(term), "");
            if (newTitle != testTitle)
            {
                testTitle = newTitle;
                return true;
            }

            newTitle = testTitle.Replace(RemoveRepeats(term), "");
            if (newTitle != testTitle)
            {
                testTitle = newTitle;
                return true;
            }

            return false;
        });

    public bool HasNoBannedTerms(string targetTitle, string testTitle)
    {
        var normalisedTitle = targetTitle.NormalizeTitle();

        var normalisedWords = normalisedTitle.Split(' ');

        var hasBannedWords = normalisedWords.Where(word => word.Length >= 3).Any(word => !targetTitle.Contains(word) && _wordCollections.AdultWords.Contains(word));

        var hasCompounds = _compoundWords.Any(term => normalisedTitle.Contains(term, StringComparison.OrdinalIgnoreCase));

        return !hasBannedWords &&
               !hasCompounds;
    }

    public bool HasNoBannedTerms(string targetTitle)
    {
        var normalisedTitle = targetTitle.NormalizeTitle();

        var normalisedWords = normalisedTitle.Split(' ');

        var hasBannedWords = normalisedWords.Where(word => word.Length >= 3).Any(word => normalisedWords.Contains(word, StringComparer.OrdinalIgnoreCase) && _wordCollections.AdultWords.Contains(word));

        var hasCompounds = _compoundWords.Any(term => normalisedTitle.Contains(term, StringComparison.OrdinalIgnoreCase));

       return !hasBannedWords &&
                      !hasCompounds;
    }

    public bool MeetsTitleConditions(string targetTitle, List<string> years, string testTitle) => MatchesTitle(targetTitle, years, testTitle) && HasNoBannedTerms(targetTitle, testTitle);

    public int CountUncommonWords(string title)
    {
        var processedTitle = WhitespaceMatcher().Split(title)
            .Select(word => WordProcessingMatcher().Replace(word.ToLower(), ""))
            .Where(word => word.Length > 3)
            .ToList();

        return processedTitle.Count(word => !_wordCollections.CommonWords.Contains(word));
    }

    public ParsedFilename Parse(string name) => _torrentTitleParser.Parse(name);

    public TorrentType GetTypeByName(string name) => _torrentTitleParser.GetTypeByName(name);
}
