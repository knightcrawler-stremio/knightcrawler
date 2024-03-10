namespace Producer.Features.ParseTorrentTitle;

public static partial class QualityParser
{
    [GeneratedRegex(@"\b(?<proper>proper|repack|rerip)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex ProperRegex();

    [GeneratedRegex(@"\b(?<real>REAL)\b", RegexOptions.None, "en-GB")]
    private static partial Regex RealRegex();

    [GeneratedRegex(@"(?<version>v\d\b|\[v\d\])", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex VersionExp();

    [GeneratedRegex(@"\b(?<remux>(BD|UHD)?Remux)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex RemuxExp();

    [GeneratedRegex(@"\b(COMPLETE|ISO|BDISO|BDMux|BD25|BD50|BR.?DISK)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex BdiskExp();

    [GeneratedRegex(@"\b(?<rawhd>RawHD|1080i[-_. ]HDTV|Raw[-_. ]HD|MPEG[-_. ]?2)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex RawHdExp();

    [GeneratedRegex(@"hr[-_. ]ws", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex HighDefPdtvRegex();

    public static void Parse(string title, out QualityModel result)
    {
        var normalizedTitle = title.Trim().Replace("_", " ").Replace("[", " ").Replace("]", " ").Trim().ToLower();

        ParseQualityModifyers(title, out var revision);
        ResolutionParser.Parse(normalizedTitle, out var resolution, out _);
        SourceParser.ParseSourceGroups(normalizedTitle, out var sourceGroups);
        SourceParser.Parse(normalizedTitle, out var source);
        VideoCodecsParser.Parse(normalizedTitle, out var codec, out _);

        result = new()
        {
            Sources = source,
            Resolution = resolution,
            Revision = revision,
            Modifier = null,
        };

        if (BdiskExp().IsMatch(normalizedTitle) && sourceGroups["bluray"])
        {
            result.Modifier = QualityModifier.BRDISK;
            result.Sources = [Source.BLURAY];
        }

        if (RemuxExp().IsMatch(normalizedTitle) && !sourceGroups["webdl"] && !sourceGroups["hdtv"])
        {
            result.Modifier = QualityModifier.REMUX;
            result.Sources = [Source.BLURAY];
        }

        if (RawHdExp().IsMatch(normalizedTitle) && result.Modifier != QualityModifier.BRDISK && result.Modifier != QualityModifier.REMUX)
        {
            result.Modifier = QualityModifier.RAWHD;
            result.Sources = [Source.TV];
        }

        if (sourceGroups["bluray"])
        {
            result.Sources = [Source.BLURAY];

            if (codec == VideoCodec.XVID)
            {
                result.Resolution = Resolution.R480P;
                result.Sources = [Source.DVD];
            }

            if (resolution == null)
            {
                // assume bluray is at least 720p
                result.Resolution = Resolution.R720P;
            }

            if (resolution == null && result.Modifier == QualityModifier.BRDISK)
            {
                result.Resolution = Resolution.R1080P;
            }

            if (resolution == null && result.Modifier == QualityModifier.REMUX)
            {
                result.Resolution = Resolution.R2160P;
            }

            return;
        }

        if (sourceGroups["webdl"] || sourceGroups["webrip"])
        {
            result.Sources = source;

            if (resolution == null)
            {
                result.Resolution = Resolution.R480P;
            }

            if (resolution == null)
            {
                result.Resolution = Resolution.R480P;
            }

            if (resolution == null && title.Contains("[WEBDL]"))
            {
                result.Resolution = Resolution.R720P;
            }

            return;
        }

        if (sourceGroups["hdtv"])
        {
            result.Sources = [Source.TV];

            if (resolution == null)
            {
                result.Resolution = Resolution.R480P;
            }

            if (resolution == null && title.Contains("[HDTV]"))
            {
                result.Resolution = Resolution.R720P;
            }

            return;
        }

        if (sourceGroups["pdtv"] || sourceGroups["sdtv"] || sourceGroups["dsr"] || sourceGroups["tvrip"])
        {
            result.Sources = [Source.TV];

            if (HighDefPdtvRegex().IsMatch(normalizedTitle))
            {
                result.Resolution = Resolution.R720P;
                return;
            }

            result.Resolution = Resolution.R480P;
            return;
        }

        if (sourceGroups["bdrip"] || sourceGroups["brrip"])
        {
            if (codec == VideoCodec.XVID)
            {
                result.Resolution = Resolution.R480P;
                result.Sources = [Source.DVD];
                return;
            }

            if (resolution == null)
            {
                // bdrips are at least 480p
                result.Resolution = Resolution.R480P;
            }

            result.Sources = [Source.BLURAY];
            return;
        }

        if (sourceGroups["workprint"])
        {
            result.Sources = [Source.WORKPRINT];
            return;
        }

        if (sourceGroups["cam"])
        {
            result.Sources = [Source.CAM];
            return;
        }

        if (sourceGroups["ts"])
        {
            result.Sources = [Source.TELESYNC];
            return;
        }

        if (sourceGroups["tc"])
        {
            result.Sources = [Source.TELECINE];
            return;
        }

        if (result.Modifier == null && (resolution == Resolution.R2160P || resolution == Resolution.R1080P || resolution == Resolution.R720P))
        {
            result.Sources = [Source.WEBDL];
        }
    }

    private static void ParseQualityModifyers(string title, out Revision revision)
    {
        var normalizedTitle = title.Trim().Replace("_", " ").Trim().ToLower();

        revision = new()
        {
            Version = 1,
            Real = 0,
        };

        if (ProperRegex().IsMatch(normalizedTitle))
        {
            revision.Version = 2;
        }

        var versionResult = VersionExp().Match(normalizedTitle);
        if (versionResult.Success)
        {
            // get numbers from version regex
            var digits = Regex.Match(versionResult.Groups["version"].Value, @"\d");
            if (digits.Success)
            {
                var value = int.Parse(digits.Value);
                revision.Version = value;
            }
        }

        var realCount = 0;
        var realGlobalExp = new Regex(RealRegex().ToString(), RegexOptions.None);
        // use non normalized title to prevent insensitive REAL matching
        while (realGlobalExp.IsMatch(title))
        {
            realCount += 1;
        }

        revision.Real = realCount;
    }
}