namespace Producer.Features.PTN2;

public static class Extras
{
    public const string Delimiters = @"[\.\s\-\+_\/(),]";
    
    public static List<Tuple<string, string>> Langs =
    [
        new("rus(?:sian)?", "Russian"),
        new("(?:True)?fre?(?:nch)?", "French"),
        new("(?:nu)?ita(?:liano?)?", "Italian"),
        new("castellano|spa(?:nish)?|esp?", "Spanish"),
        new("swedish", "Swedish"),
        new("dk|dan(?:ish)?", "Danish"),
        new("ger(?:man)?|deu(?:tsch)?", "German"),
        new("nordic", "Nordic"),
        new("exyu", "ExYu"),
        new("chs|chi(?:nese)?", "Chinese"),
        new("hin(?:di)?", "Hindi"),
        new("polish|poland|pl", "Polish"),
        new("mandarin", "Mandarin"),
        new("kor(?:ean)?", "Korean"),
        new("ben(?:gali)?|bangla", "Bengali"),
        new("kan(?:nada)?", "Kannada"),
        new("tam(?:il)?", "Tamil"),
        new("tel(?:ugu)?", "Telugu"),
        new("mar(?:athi)?", "Marathi"),
        new("mal(?:ayalam)?", "Malayalam"),
        new("japanese|ja?p", "Japanese"),
        new("interslavic", "Interslavic"),
        new("ara(?:bic)?", "Arabic"),
        new("urdu", "Urdu"),
        new("punjabi", "Punjabi"),
        new("portuguese", "Portuguese"),
        new("albanian?", "Albanian"),
        new("egypt(?:ian)?", "Egyptian"),
        new("en?(?:g(?:lish)?)?", "English"),
    ];
    
    public static List<Tuple<string, string>> Genres =
    [
        new("Sci-?Fi", "Sci-Fi"),
        new("Drama", "Drama"),
        new("Comedy", "Comedy"),
        new("West(?:\\.|ern)?", "Western"),
        new("Action", "Action"),
        new("Adventure", "Adventure"),
        new("Thriller", "Thriller"),
    ];
    
    public static List<string> CompleteSeries =
    [
        "(?:the\\s)?complete\\s(?:series|season|collection)$",
        "(?:the)\\scomplete\\s?(?:series|season|collection)?$",
    ];
    
    public static List<Dictionary<string, string>> Exceptions =
    [
        new()
        {
            {"parsed_title", "Marvel's Agents of S H I E L D"},
            {"incorrect_parse", "Marvel's Agents of S H I E L D"},
            {"actual_title", "Marvel's Agents of S.H.I.E.L.D."}
        },
        new()
        {
            {"parsed_title", "Marvels Agents of S H I E L D"},
            {"incorrect_parse", "Marvels Agents of S H I E L D"},
            {"actual_title", "Marvel's Agents of S.H.I.E.L.D."}
        },
        new()
        {
            {"parsed_title", "Magnum P I"},
            {"incorrect_parse", "Magnum P I"},
            {"actual_title", "Magnum P.I."},
        },
    ];

    public static Dictionary<string, List<string>> PatternsIgnoreTitle = new()
    {
        {"language", []},
        {"audio", ["LiNE"]},
        {"network", ["Hallmark"]},
        {"untouched", []},
        {"internal", []},
        {"limited", []},
        {"proper", []},
        {"extended", []},
    };

    public static List<Tuple<int, int>> Channels =
    [
        new(1, 0),
        new(2, 0),
        new(5, 0),
        new(5, 1),
        new(6, 1),
        new(7, 1),
    ];
    
    public static List<Patterns.PatternRecord> GetChannelAudioOptions(List<Patterns.PatternRecord> patternsWithNames)
    {
        var options = new List<Patterns.PatternRecord>();
        foreach (var patternRecord in patternsWithNames)
        {
            foreach (var (speakers, subwoofers) in Channels)
            {
                options.Add(new(
                    $"((?:{patternRecord.Pattern}){Delimiters}*{speakers}[. \\-]?{subwoofers}(?:ch)?)",
                    $"{patternRecord.Name} {speakers}.{subwoofers}",
                    patternRecord.Transformers
                ));
            }
            options.Add(patternRecord with
            {
                Pattern = $"({patternRecord.Pattern})",
            });
        }
        return options;
    }

    public static List<Patterns.PatternRecord> PrefixPatternWith(List<string> prefixes, List<Patterns.PatternRecord> patternOptions, string between = "", bool optional = false)
    {
        var optionalChar = optional ? "?" : "";
        var options = new List<Patterns.PatternRecord>();
        foreach (var prefix in prefixes)
        {
            options.AddRange(patternOptions.Select(patternRecord => 
                patternRecord with
                {
                    Pattern = $"(?:{prefix}){optionalChar}(?:{between})?({patternRecord.Pattern})",
                }));
        }
        return options;
    }

    public static List<Patterns.PatternRecord> SuffixPatternWith(List<string> suffixes, List<Patterns.PatternRecord> patternOptions, string between = "", bool optional = false)
    {
        var optionalChar = optional ? "?" : "";
        var options = new List<Patterns.PatternRecord>();
        foreach (var suffix in suffixes)
        {
            options.AddRange(
                patternOptions.Select(
                    patternRecord => patternRecord with
                    {
                        Pattern = $"({patternRecord.Pattern})(?:{between})?(?:{suffix}){optionalChar}",
                    }));
        }
        return options;
    }

    public static string LinkPatterns(List<Patterns.PatternRecord> patternOptions) => $"(?:{string.Join("|", patternOptions.Select(po => po.Pattern))})";
    public static string LinkPatterns(List<Tuple<string, string>> patternOptions) => $"(?:{string.Join("|", patternOptions.Select(po => po.Item1))})";
}