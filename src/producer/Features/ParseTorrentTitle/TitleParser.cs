namespace Producer.Features.ParseTorrentTitle;

public static partial class TitleParser
{
    [GeneratedRegex(@"^(?<title>(?![([]).+?)?(?:(?:[-_\W](?<![)[!]))*\(?\b(?<edition>(((Extended.|Ultimate.)?(Director.?s|Collector.?s|Theatrical|Anniversary|The.Uncut|Ultimate|Final(?=(.(Cut|Edition|Version)))|Extended|Rogue|Special|Despecialized|\d{2,3}(th)?.Anniversary)(.(Cut|Edition|Version))?(.(Extended|Uncensored|Remastered|Unrated|Uncut|IMAX|Fan.?Edit))?|((Uncensored|Remastered|Unrated|Uncut|IMAX|Fan.?Edit|Edition|Restored|((2|3|4)in1))))))\b\)?.{1,3}(?<year>(1(8|9)|20)\d{2}(?!p|i|\d+|\]|\W\d+)))+(\W+|_|$)(?!\\)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MovieTitleYearRegex1();

    [GeneratedRegex(@"^(?<title>(?![([]).+?)?(?:(?:[-_\W](?<![)[!]))*\((?<year>(1(8|9)|20)\d{2}(?!p|i|(1(8|9)|20)\d{2}|\]|\W(1(8|9)|20)\d{2})))+", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MovieTitleYearRegex2();

    [GeneratedRegex(@"^(?<title>(?![([]).+?)?(?:(?:[-_\W](?<![)[!]))*(?<year>(1(8|9)|20)\d{2}(?!p|i|(1(8|9)|20)\d{2}|\]|\W(1(8|9)|20)\d{2})))+(\W+|_|$)(?!\\)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MovieTitleYearRegex3();

    [GeneratedRegex(@"^(?<title>.+?)?(?:(?:[-_\W](?<![()[!]))*(?<year>(\[\w *\])))+(\W+|_|$)(?!\\)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MovieTitleYearRegex4();

    [GeneratedRegex(@"^(?<title>(?![([]).+?)?(?:(?:[-_\W](?<![)!]))*(?<year>(1(8|9)|20)\d{2}(?!p|i|\d+|\W\d+)))+(\W+|_|$)(?!\\)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MovieTitleYearRegex5();

    [GeneratedRegex(@"^(?<title>.+?)?(?:(?:[-_\W](?<![)[!]))*(?<year>(1(8|9)|20)\d{2}(?!p|i|\d+|\]|\W\d+)))+(\W+|_|$)(?!\\)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MovieTitleYearRegex6();

    [GeneratedRegex(@"\s*(?:480[ip]|576[ip]|720[ip]|1080[ip]|2160[ip]|HVEC|[xh][\W_]?26[45]|DD\W?5\W1|[<>?*:|]|848x480|1280x720|1920x1080)((8|10)b(it))?", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SimpleTitleRegex();

    [GeneratedRegex(@"^\[\s*[a-z]+(\.[a-z]+)+\s*\][- ]*|^www\.[a-z]+\.(?:com|net)[ -]*", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex WebsitePrefixRegex();

    [GeneratedRegex(@"^\[(?:REQ)\]", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex CleanTorrentPrefixRegex();

    [GeneratedRegex(@"\[(?:ettv|rartv|rarbg|cttv)\]$", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex CleanTorrentSuffixRegex();

    [GeneratedRegex(@"\b(Bluray|(dvdr?|BD)rip|HDTV|HDRip|TS|R5|CAM|SCR|(WEB|DVD)?.?SCREENER|DiVX|xvid|web-?dl)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex CommonSourcesRegex();

    [GeneratedRegex(@"\b(?<webdl>WEB[-_. ]DL|HDRIP|WEBDL|WEB-DLMux|NF|APTV|NETFLIX|NetflixU?HD|DSNY|DSNP|HMAX|AMZN|AmazonHD|iTunesHD|MaxdomeHD|WebHD|WEB$|[. ]WEB[. ](?:[xh]26[45]|DD5[. ]1)|\d+0p[. ]WEB[. ]|\b\s\/\sWEB\s\/\s\b|AMZN[. ]WEB[. ])\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex WebdlExp();

    [GeneratedRegex(@"\[.+?\]", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex RequestInfoRegex();

    [GeneratedRegex(
        @"\b((Extended.|Ultimate.)?(Director.?s|Collector.?s|Theatrical|Anniversary|The.Uncut|DC|Ultimate|Final(?=(.(Cut|Edition|Version)))|Extended|Special|Despecialized|unrated|\d{2,3}(th)?.Anniversary)(.(Cut|Edition|Version))?(.(Extended|Uncensored|Remastered|Unrated|Uncut|IMAX|Fan.?Edit))?|((Uncensored|Remastered|Unrated|Uncut|IMAX|Fan.?Edit|Edition|Restored|((2|3|4)in1)))){1,3}",
        RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex EditionExp();

    [GeneratedRegex(@"\b(TRUE.?FRENCH|videomann|SUBFRENCH|PLDUB|MULTI)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex LanguageExp();

    [GeneratedRegex(@"\b(PROPER|REAL|READ.NFO)", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SceneGarbageExp();

    [GeneratedRegex(@"-([a-z0-9]+)$", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex GrouplessTitleRegex();

    public static void Parse(string title, out string parsedTitle, out string? year)
    {
        var simpleTitle = SimplifyTitle(title);

        // Removing the group from the end could be trouble if a title is "title-year"
        var grouplessTitle = simpleTitle.Replace(GrouplessTitleRegex().ToString(), "");

        var movieTitleYearRegex = new List<Regex>
        {
            MovieTitleYearRegex1(), MovieTitleYearRegex2(), MovieTitleYearRegex3(), MovieTitleYearRegex4(), MovieTitleYearRegex5(),
            MovieTitleYearRegex6()
        };

        foreach (var exp in movieTitleYearRegex)
        {
            var match = exp.Match(grouplessTitle);

            if (match.Success)
            {
                parsedTitle = ReleaseTitleCleaner(match.Groups["title"].Value);

                year = match.Groups["year"].Value;

                return;
            }
        }

        // year not found, attack using codec or resolution
        // attempt to parse using the first found artifact like codec
        ResolutionParser.Parse(title, out var resolution, out _);
        VideoCodecsParser.Parse(title, out var videoCodec, out _);
        AudioChannelsParser.Parse(title, out var channels, out _);
        AudioCodecsParser.Parse(title, out var audioCodec, out _);
        var resolutionPosition = title.IndexOf(resolution?.Value ?? string.Empty, StringComparison.Ordinal);
        var videoCodecPosition = title.IndexOf(videoCodec?.Value ?? string.Empty, StringComparison.Ordinal);
        var channelsPosition = title.IndexOf(channels?.Value ?? string.Empty, StringComparison.Ordinal);
        var audioCodecPosition = title.IndexOf(audioCodec?.Value ?? string.Empty, StringComparison.Ordinal);
        var positions = new List<int> {resolutionPosition, audioCodecPosition, channelsPosition, videoCodecPosition}.Where(x => x > 0).ToList();

        if (positions.Count != 0)
        {
            var firstPosition = positions.Min();
            parsedTitle = ReleaseTitleCleaner(title[..firstPosition]);
            year = null;
            return;
        }

        parsedTitle = title.Trim();
        year = null;
    }

    public static string SimplifyTitle(string title)
    {
        var simpleTitle = title.Replace(SimpleTitleRegex().ToString(), "");
        simpleTitle = simpleTitle.Replace(WebsitePrefixRegex().ToString(), "");
        simpleTitle = simpleTitle.Replace(CleanTorrentPrefixRegex().ToString(), "");
        simpleTitle = simpleTitle.Replace(CleanTorrentSuffixRegex().ToString(), "");
        simpleTitle = simpleTitle.Replace(CommonSourcesRegex().ToString(), "");
        simpleTitle = simpleTitle.Replace(WebdlExp().ToString(), "");

        // allow filtering of up to two codecs.
        // maybe parseVideoCodec should be an array
        VideoCodecsParser.Parse(simpleTitle, out _, out var source1);

        if (!string.IsNullOrEmpty(source1))
        {
            simpleTitle = simpleTitle.Replace(source1, "");
        }

        VideoCodecsParser.Parse(simpleTitle, out _, out var source2);

        if (!string.IsNullOrEmpty(source2))
        {
            simpleTitle = simpleTitle.Replace(source2, "");
        }

        return simpleTitle.Trim();
    }

    public static string ReleaseTitleCleaner(string title)
    {
        if (string.IsNullOrEmpty(title) || title.Length == 0 || title == "(")
        {
            return null;
        }

        var trimmedTitle = title.Replace("_", " ");
        trimmedTitle = trimmedTitle.Replace(RequestInfoRegex().ToString(), "").Trim();
        trimmedTitle = trimmedTitle.Replace(CommonSourcesRegex().ToString(), "").Trim();
        trimmedTitle = trimmedTitle.Replace(WebdlExp().ToString(), "").Trim();
        trimmedTitle = trimmedTitle.Replace(EditionExp().ToString(), "").Trim();
        trimmedTitle = trimmedTitle.Replace(LanguageExp().ToString(), "").Trim();
        trimmedTitle = trimmedTitle.Replace(SceneGarbageExp().ToString(), "").Trim();

        trimmedTitle = Language.List.Aggregate(trimmedTitle, (current, lang) => current.Replace($@"\b{lang.Value.ToUpper()}", "").Trim());

        // Look for gap formed by removing items
        trimmedTitle = trimmedTitle.Split("  ")[0];
        trimmedTitle = trimmedTitle.Split("..")[0];

        var parts = trimmedTitle.Split('.');
        var result = "";
        var n = 0;
        var previousAcronym = false;
        var nextPart = "";

        foreach (var part in parts)
        {
            if (parts.Length >= n + 2)
            {
                nextPart = parts[n + 1];
            }

            if (part.Length == 1 && part.ToLower() != "a" && !int.TryParse(part, out _))
            {
                result += part + ".";
                previousAcronym = true;
            }
            else if (part.ToLower() == "a" && (previousAcronym || nextPart.Length == 1))
            {
                result += part + ".";
                previousAcronym = true;
            }
            else
            {
                if (previousAcronym)
                {
                    result += " ";
                    previousAcronym = false;
                }

                result += part + " ";
            }

            n++;
        }

        return result.Trim();
    }
}
