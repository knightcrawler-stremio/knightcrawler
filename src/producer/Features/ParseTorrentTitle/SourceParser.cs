namespace Producer.Features.ParseTorrentTitle;

public static partial class SourceParser
{
    [GeneratedRegex(@"\b(?<bluray>M?Blu-?Ray|HDDVD|BD|UHDBD|BDISO|BDMux|BD25|BD50|BR.?DISK|Bluray(1080|720)p?|BD(1080|720)p?)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex BlurayExp();

    [GeneratedRegex(@"\b(?<webdl>WEB[-_. ]DL|HDRIP|WEBDL|WEB-DLMux|NF|APTV|NETFLIX|NetflixU?HD|DSNY|DSNP|HMAX|AMZN|AmazonHD|iTunesHD|MaxdomeHD|WebHD|WEB$|[. ]WEB[. ](?:[xh]26[45]|DD5[. ]1)|\d+0p[. ]WEB[. ]|\b\s\/\sWEB\s\/\s\b|AMZN[. ]WEB[. ])\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex WebdlExp();

    [GeneratedRegex(@"\b(?<webrip>WebRip|Web-Rip|WEBCap|WEBMux)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex WebripExp();

    [GeneratedRegex(@"\b(?<hdtv>HDTV)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex HdtvExp();

    [GeneratedRegex(@"\b(?<bdrip>BDRip)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex BdripExp();

    [GeneratedRegex(@"\b(?<brrip>BRRip)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex BrripExp();

    [GeneratedRegex(@"\b(?<scr>SCR|SCREENER|DVDSCR|(DVD|WEB).?SCREENER)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex ScrExp();

    [GeneratedRegex(@"\b(?<dvdr>DVD-R|DVDR)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex DvdrExp();

    [GeneratedRegex(@"\b(?<dvd>DVD9?|DVDRip|NTSC|PAL|xvidvd|DvDivX)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex DvdExp();

    [GeneratedRegex(@"\b(?<dsr>WS[-_. ]DSR|DSR)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex DsrExp();

    [GeneratedRegex(@"\b(?<regional>R[0-9]{1}|REGIONAL)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex RegionalExp();

    [GeneratedRegex(@"\b(?<ppv>PPV)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex PpvExp();

    [GeneratedRegex(@"\b(?<ts>TS|TELESYNC|HD-TS|HDTS|PDVD|TSRip|HDTSRip)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex TsExp();

    [GeneratedRegex(@"\b(?<tc>TC|TELECINE|HD-TC|HDTC)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex TcExp();

    [GeneratedRegex(@"\b(?<cam>CAMRIP|CAM|HDCAM|HD-CAM)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex CamExp();

    [GeneratedRegex(@"\b(?<workprint>WORKPRINT|WP)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex WorkprintExp();

    [GeneratedRegex(@"\b(?<pdtv>PDTV)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex PdtvExp();

    [GeneratedRegex(@"\b(?<sdtv>SDTV)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SdtvExp();

    [GeneratedRegex(@"\b(?<tvrip>TVRip)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex TvripExp();

    public static void Parse(string title, out List<Source> result)
    {
        ParseSourceGroups(title, out var groups);

        result = [];

        if (groups["bluray"] || groups["bdrip"] || groups["brrip"])
        {
            result.Add(Source.BLURAY);
        }

        if (groups["webrip"])
        {
            result.Add(Source.WEBRIP);
        }

        if (!groups["webrip"] && groups["webdl"])
        {
            result.Add(Source.WEBDL);
        }

        if (groups["dvdr"] || (groups["dvd"] && !groups["scr"]))
        {
            result.Add(Source.DVD);
        }

        if (groups["ppv"])
        {
            result.Add(Source.PPV);
        }

        if (groups["workprint"])
        {
            result.Add(Source.WORKPRINT);
        }

        if (groups["pdtv"] || groups["sdtv"] || groups["dsr"] || groups["tvrip"] || groups["hdtv"])
        {
            result.Add(Source.TV);
        }

        if (groups["cam"])
        {
            result.Add(Source.CAM);
        }

        if (groups["ts"])
        {
            result.Add(Source.TELESYNC);
        }

        if (groups["tc"])
        {
            result.Add(Source.TELECINE);
        }

        if (groups["scr"])
        {
            result.Add(Source.SCREENER);
        }
    }

    public static void ParseSourceGroups(string title, out Dictionary<string, bool> groups)
    {
        var normalizedName = title.Replace("_", " ").Replace("[", " ").Replace("]", " ").Trim();

        groups = new()
        {
            {"bluray", BlurayExp().IsMatch(normalizedName)},
            {"webdl", WebdlExp().IsMatch(normalizedName)},
            {"webrip", WebripExp().IsMatch(normalizedName)},
            {"hdtv", HdtvExp().IsMatch(normalizedName)},
            {"bdrip", BdripExp().IsMatch(normalizedName)},
            {"brrip", BrripExp().IsMatch(normalizedName)},
            {"scr", ScrExp().IsMatch(normalizedName)},
            {"dvdr", DvdrExp().IsMatch(normalizedName)},
            {"dvd", DvdExp().IsMatch(normalizedName)},
            {"dsr", DsrExp().IsMatch(normalizedName)},
            {"regional", RegionalExp().IsMatch(normalizedName)},
            {"ppv", PpvExp().IsMatch(normalizedName)},
            {"ts", TsExp().IsMatch(normalizedName)},
            {"tc", TcExp().IsMatch(normalizedName)},
            {"cam", CamExp().IsMatch(normalizedName)},
            {"workprint", WorkprintExp().IsMatch(normalizedName)},
            {"pdtv", PdtvExp().IsMatch(normalizedName)},
            {"sdtv", SdtvExp().IsMatch(normalizedName)},
            {"tvrip", TvripExp().IsMatch(normalizedName)},
        };
    }
}