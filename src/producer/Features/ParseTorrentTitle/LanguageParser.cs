namespace Producer.Features.ParseTorrentTitle;

public static partial class LanguageParser
{
    [GeneratedRegex(@"\bWEB-?DL\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex WebDL();
    
    [GeneratedRegex(@"(?<!(WEB-))\b(MULTi|DUAL|DL)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MultiExp();
    
    [GeneratedRegex(@"\b(english|eng|EN|FI)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex EnglishRegex();

    [GeneratedRegex(@"\b(DK|DAN|danish)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex DanishRegex();

    [GeneratedRegex(@"\b(SE|SWE|swedish)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SwedishRegex();

    [GeneratedRegex(@"\b(ice|Icelandic)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex IcelandicRegex();

    [GeneratedRegex(@"\b(chi|chinese)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex ChineseRegex();

    [GeneratedRegex(@"\b(ita|italian)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex ItalianRegex();

    [GeneratedRegex(@"\b(german|videomann)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex GermanRegex();

    [GeneratedRegex(@"\b(flemish)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex FlemishRegex();

    [GeneratedRegex(@"\b(greek)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex GreekRegex();

    [GeneratedRegex(@"\b(FR|FRENCH|VOSTFR|VO|VFF|VFQ|VF2|TRUEFRENCH|SUBFRENCH)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex FrenchRegex();

    [GeneratedRegex(@"\b(russian|rus)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex RussianRegex();

    [GeneratedRegex(@"\b(norwegian|NO)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex NorwegianRegex();

    [GeneratedRegex(@"\b(HUNDUB|HUN|hungarian)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex HungarianRegex();

    [GeneratedRegex(@"\b(HebDub)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex HebrewRegex();

    [GeneratedRegex(@"\b(CZ|SK)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex CzechRegex();

    [GeneratedRegex(@"(?<ukrainian>\bukr\b)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex UkrainianRegex();

    [GeneratedRegex(@"\b(PL|PLDUB|POLISH)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex PolishRegex();

    [GeneratedRegex(@"\b(nl|dutch)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex DutchRegex();

    [GeneratedRegex(@"\b(HIN|Hindi)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex HindiRegex();

    [GeneratedRegex(@"\b(TAM|Tamil)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex TamilRegex();

    [GeneratedRegex(@"\b(Arabic)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex ArabicRegex();

    [GeneratedRegex(@"\b(Latvian)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex LatvianRegex();

    [GeneratedRegex(@"\b(Lithuanian)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex LithuanianRegex();

    [GeneratedRegex(@"\b(RO|Romanian|rodubbed)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex RomanianRegex();

    [GeneratedRegex(@"\b(SK|Slovak)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SlovakRegex();

    [GeneratedRegex(@"\b(Brazilian)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex BrazilianRegex();

    [GeneratedRegex(@"\b(Persian)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex PersianRegex();

    [GeneratedRegex(@"\b(Bengali)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex BengaliRegex();

    [GeneratedRegex(@"\b(Bulgarian)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex BulgarianRegex();

    [GeneratedRegex(@"\b(Serbian)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SerbianRegex();

    public static void Parse(string title, out List<Language> languages)
    {
        TitleParser.Parse(title, out var parsedTitle, out _);
        
        var languageTitle = title.Replace(".", " ").Replace(parsedTitle, "").ToLower();

        languages = new();

        if (languageTitle.Contains("spanish"))
        {
            languages.Add(Language.Spanish);
        }

        if (languageTitle.Contains("japanese"))
        {
            languages.Add(Language.Japanese);
        }

        if (languageTitle.Contains("cantonese"))
        {
            languages.Add(Language.Cantonese);
        }

        if (languageTitle.Contains("mandarin"))
        {
            languages.Add(Language.Mandarin);
        }

        if (languageTitle.Contains("korean"))
        {
            languages.Add(Language.Korean);
        }

        if (languageTitle.Contains("vietnamese"))
        {
            languages.Add(Language.Vietnamese);
        }

        if (languageTitle.Contains("finnish"))
        {
            languages.Add(Language.Finnish);
        }

        if (languageTitle.Contains("turkish"))
        {
            languages.Add(Language.Turkish);
        }

        if (languageTitle.Contains("portuguese"))
        {
            languages.Add(Language.Portuguese);
        }

        if (languageTitle.Contains("hebrew"))
        {
            languages.Add(Language.Hebrew);
        }

        if (languageTitle.Contains("czech"))
        {
            languages.Add(Language.Czech);
        }

        if (languageTitle.Contains("ukrainian"))
        {
            languages.Add(Language.Ukrainian);
        }

        if (languageTitle.Contains("catalan"))
        {
            languages.Add(Language.Catalan);
        }

        if (languageTitle.Contains("estonian"))
        {
            languages.Add(Language.Estonian);
        }

        if (languageTitle.Contains("thai"))
        {
            languages.Add(Language.Thai);
        }

        if (EnglishRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.English);
        }

        if (DanishRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Danish);
        }

        if (SwedishRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Swedish);
        }

        if (IcelandicRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Icelandic);
        }

        if (ChineseRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Chinese);
        }

        if (ItalianRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Italian);
        }

        if (GermanRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.German);
        }

        if (FlemishRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Flemish);
        }

        if (GreekRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Greek);
        }

        if (FrenchRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.French);
        }

        if (RussianRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Russian);
        }

        if (NorwegianRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Norwegian);
        }

        if (HungarianRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Hungarian);
        }

        if (HebrewRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Hebrew);
        }

        if (CzechRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Czech);
        }

        if (UkrainianRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Ukrainian);
        }

        if (PolishRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Polish);
        }

        if (DutchRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Dutch);
        }

        if (HindiRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Hindi);
        }

        if (TamilRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Tamil);
        }

        if (ArabicRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Arabic);
        }

        if (LatvianRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Latvian);
        }

        if (LithuanianRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Lithuanian);
        }

        if (RomanianRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Romanian);
        }

        if (SlovakRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Slovak);
        }

        if (BrazilianRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Brazilian);
        }

        if (PersianRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Persian);
        }

        if (BengaliRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Bengali);
        }

        if (BulgarianRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Bulgarian);
        }

        if (SerbianRegex().IsMatch(languageTitle))
        {
            languages.Add(Language.Serbian);
        }
    }
    
    public static bool? IsMulti(string title)
    {
        var noWebTitle = WebDL().Replace(title, "");
        return MultiExp().IsMatch(noWebTitle) ? true : null;
    }
}