namespace Producer.Features.ParseTorrentTitle;

public static partial class EditionParser
{
    [GeneratedRegex(@"\b(INTERNAL)\b", RegexOptions.IgnoreCase)]
    private static partial Regex InternalExp();

    [GeneratedRegex(@"\b(Remastered|Anniversary|Restored)\b", RegexOptions.IgnoreCase)]
    private static partial Regex RemasteredExp();

    [GeneratedRegex(@"\b(IMAX)\b", RegexOptions.IgnoreCase)]
    private static partial Regex ImaxExp();

    [GeneratedRegex(@"\b(Uncensored|Unrated)\b", RegexOptions.IgnoreCase)]
    private static partial Regex UnratedExp();

    [GeneratedRegex(@"\b(Extended|Uncut|Ultimate|Rogue|Collector)\b", RegexOptions.IgnoreCase)]
    private static partial Regex ExtendedExp();

    [GeneratedRegex(@"\b(Theatrical)\b", RegexOptions.IgnoreCase)]
    private static partial Regex TheatricalExp();

    [GeneratedRegex(@"\b(Directors?)\b", RegexOptions.IgnoreCase)]
    private static partial Regex DirectorsExp();

    [GeneratedRegex(@"\b(Despecialized|Fan.?Edit)\b", RegexOptions.IgnoreCase)]
    private static partial Regex FanExp();

    [GeneratedRegex(@"\b(LIMITED)\b", RegexOptions.IgnoreCase)]
    private static partial Regex LimitedExp();

    [GeneratedRegex(@"\b(HDR)\b", RegexOptions.IgnoreCase)]
    private static partial Regex HdrExp();

    [GeneratedRegex(@"\b(3D)\b", RegexOptions.IgnoreCase)]
    private static partial Regex ThreeD();

    [GeneratedRegex(@"\b(Half-?SBS|HSBS)\b", RegexOptions.IgnoreCase)]
    private static partial Regex Hsbs();

    [GeneratedRegex(@"\b((?<!H|HALF-)SBS)\b", RegexOptions.IgnoreCase)]
    private static partial Regex Sbs();

    [GeneratedRegex(@"\b(HOU)\b", RegexOptions.IgnoreCase)]
    private static partial Regex Hou();

    [GeneratedRegex(@"\b(UHD)\b", RegexOptions.IgnoreCase)]
    private static partial Regex Uhd();

    [GeneratedRegex(@"\b(OAR)\b", RegexOptions.IgnoreCase)]
    private static partial Regex Oar();

    [GeneratedRegex(@"\b(DV(\b(HDR10|HLG|SDR))?)\b", RegexOptions.IgnoreCase)]
    private static partial Regex DolbyVision();

    [GeneratedRegex(@"\b((?<hcsub>(\w+(?<!SOFT|HORRIBLE)SUBS?))|(?<hc>(HC|SUBBED)))\b", RegexOptions.IgnoreCase)]
    private static partial Regex HardcodedSubsExp();

    [GeneratedRegex(@"\b((Bonus.)?Deleted.Scenes)\b", RegexOptions.IgnoreCase)]
    private static partial Regex DeletedScenes();

    [GeneratedRegex(@"\b((Bonus|Extras|Behind.the.Scenes|Making.of|Interviews|Featurettes|Outtakes|Bloopers|Gag.Reel).(?!(Deleted.Scenes)))\b", RegexOptions.IgnoreCase)]
    private static partial Regex BonusContent();

    [GeneratedRegex(@"\b(BW)\b", RegexOptions.IgnoreCase)]
    private static partial Regex Bw();

    public static Edition Parse(string title)
    {
        TitleParser.Parse(title, out var parsedTitle, out _);

        var withoutTitle = title.Replace(".", " ").Replace(parsedTitle, "").ToLower();

        var result = new Edition
        {
            Internal = InternalExp().IsMatch(withoutTitle),
            Limited = LimitedExp().IsMatch(withoutTitle),
            Remastered = RemasteredExp().IsMatch(withoutTitle),
            Extended = ExtendedExp().IsMatch(withoutTitle),
            Theatrical = TheatricalExp().IsMatch(withoutTitle),
            Directors = DirectorsExp().IsMatch(withoutTitle),
            Unrated = UnratedExp().IsMatch(withoutTitle),
            Imax = ImaxExp().IsMatch(withoutTitle),
            FanEdit = FanExp().IsMatch(withoutTitle),
            Hdr = HdrExp().IsMatch(withoutTitle),
            ThreeD = ThreeD().IsMatch(withoutTitle),
            Hsbs = Hsbs().IsMatch(withoutTitle),
            Sbs = Sbs().IsMatch(withoutTitle),
            Hou = Hou().IsMatch(withoutTitle),
            Uhd = Uhd().IsMatch(withoutTitle),
            Oar = Oar().IsMatch(withoutTitle),
            DolbyVision = DolbyVision().IsMatch(withoutTitle),
            HardcodedSubs = HardcodedSubsExp().IsMatch(withoutTitle),
            DeletedScenes = DeletedScenes().IsMatch(withoutTitle),
            BonusContent = BonusContent().IsMatch(withoutTitle),
            Bw = Bw().IsMatch(withoutTitle),
        };

        return result;
    }
}