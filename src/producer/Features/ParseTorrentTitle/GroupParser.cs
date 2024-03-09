namespace Producer.Features.ParseTorrentTitle;

public static partial class GroupParser
{
    [GeneratedRegex(@"^\[\s*[a-z]+(\.[a-z]+)+\s*\][- ]*|^www\.[a-z]+\.(?:com|net)[ -]*", RegexOptions.IgnoreCase)]
    private static partial Regex WebsitePrefixExp();

    [GeneratedRegex(@"(-(RP|1|NZBGeek|Obfuscated|Obfuscation|Scrambled|sample|Pre|postbot|xpost|Rakuv[a-z0-9]*|WhiteRev|BUYMORE|AsRequested|AlternativeToRequested|GEROV|Z0iDS3N|Chamele0n|4P|4Planet|AlteZachen|RePACKPOST))+$", RegexOptions.IgnoreCase)]
    private static partial Regex CleanReleaseGroupExp();

    [GeneratedRegex(@"-(?<releasegroup>[a-z0-9]+)(?<!WEB-DL|WEB-RIP|480p|720p|1080p|2160p|DTS-(HD|X|MA|ES)|([a-zA-Z]{3}-ENG))(?:\b|[-._ ])", RegexOptions.IgnoreCase)]
    private static partial Regex ReleaseGroupRegexExp();

    [GeneratedRegex(@"^(?:\[(?<subgroup>(?!\s).+?(?<!\s))\](?:_|-|\s|\.)?)", RegexOptions.IgnoreCase)]
    private static partial Regex AnimeReleaseGroupExp();

    [GeneratedRegex(@"(\[)?(?<releasegroup>(Joy|YIFY|YTS.(MX|LT|AG)|FreetheFish|VH-PROD|FTW-HS|DX-TV|Blu-bits|afm72|Anna|Bandi|Ghost|Kappa|MONOLITH|Qman|RZeroX|SAMPA|Silence|theincognito|D-Z0N3|t3nzin|Vyndros|HDO|DusIctv|DHD|SEV|CtrlHD|-ZR-|ADC|XZVN|RH|Kametsu|r00t|HONE))(\])?$", RegexOptions.IgnoreCase)]
    private static partial Regex ExceptionReleaseGroupRegex();

    public static string? Parse(string title)
    {
        var nowebsiteTitle = WebsitePrefixExp().Replace(title, "");
        TitleParser.Parse(nowebsiteTitle, out var releaseTitle, out _);
        releaseTitle = releaseTitle.Replace(" ", ".");
        
        var trimmed = nowebsiteTitle
            .Replace(" ", ".")
            .Replace(releaseTitle == nowebsiteTitle ? "" : releaseTitle, "")
            .Replace(".-.", ".");
        
        trimmed = TitleParser.SimplifyTitle(FileExtensionParser.RemoveFileExtension(trimmed.Trim()));

        if (trimmed.Length == 0)
        {
            return null;
        }

        var exceptionResult = ExceptionReleaseGroupRegex().Match(trimmed);

        if (exceptionResult.Groups["releasegroup"].Success)
        {
            return exceptionResult.Groups["releasegroup"].Value;
        }

        var animeResult = AnimeReleaseGroupExp().Match(trimmed);

        if (animeResult.Success)
        {
            return animeResult.Groups["subgroup"].Value;
        }

        trimmed = CleanReleaseGroupExp().Replace(trimmed, "");

        var globalReleaseGroupExp = new Regex(ReleaseGroupRegexExp().ToString(), RegexOptions.IgnoreCase);
        var result = globalReleaseGroupExp.Match(trimmed);

        while (result.Success)
        {
            if (result.Groups["releasegroup"].Success)
            {
                return result.Groups["releasegroup"].Value;
            }

            result = result.NextMatch();
        }

        return null;
    }
}