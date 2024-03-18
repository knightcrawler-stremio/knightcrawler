namespace Producer.Features.PTN2;

public class Patterns
{
    private const string SeasonRangePattern = $@"(?:Complete{Extras.Delimiters}*)?{Extras.Delimiters}*(?:s(?:easons?)?){Extras.Delimiters}*(?:s?[0-9]{{1,2}}[\s]*(?:(?:\-|(?:\s*to\s*))[\s]*s?[0-9]{{1,2}}))(?:{Extras.Delimiters}*Complete)?";
    private const string YearPattern = "(?:19[0-9]|20[0-2])[0-9]";
    private const string MonthPattern = "0[1-9]|1[0-2]";
    private const string DayPattern = "[0-2][0-9]|3[01]";
    private const string EpisodeNamePattern = $"((?:[Pp](?:ar)?t{Extras.Delimiters}*[0-9]|(?:[A-Za-z]|[0-9])[a-z]*(?:{Extras.Delimiters}|$))+)";
    private const string PreWebsiteEncoderPattern = @"[^\s\.\[\]\-\(\)]+\)\s*\[[^\s\-]+\]|[^\s\.\[\]\-\(\)]+\s*(?:-\s)?[^\s\.\[\]\-]+$";

    public readonly Dictionary<string, List<PatternRecord>> PtnPatterns;
    public readonly Dictionary<string, string> TypeMappings;

    public Patterns()
    {
        PtnPatterns = [];
        TypeMappings = SetupTypeMappings();
        AddSeasonPatterns();
        AddEpisodePatterns();
        AddDatePatterns();
        AddResolutionPatterns();
        AddQualityPatterns();
        AddNetworkPatterns();
        AddCodecPatterns();
        AddAudioPatterns();
        AddRegionPatterns();
        AddExtendedPatterns();
        AddHardcodedPatterns();
        AddProperPatterns();
        AddRepackPatterns();
        AddFpsPatterns();
        AddFiletypePatterns();
        AddWidescreenPatterns();
        AddSitePatterns();
        AddLanguageAndSubtitlePatterns();
        AddSbsPatterns();
        AddUnratedPatterns();
        AddSizePatterns();
        AddBitDepthPatterns();
        Add3DPatterns();
        AddInternalPatterns();
        AddReadNfoPatterns();
        AddHDRPatterns();
        AddDocumentaryPatterns();
        AddLimitedPatterns();
        AddRemasteredPatterns();
        AddDirectorsCutPatterns();
        AddUpscaledPatterns();
        AddUntouchedPatterns();
        AddRemuxPatterns();
        AddInternationalCutPatterns();
        AddGenrePatterns();
    }

    public record TransformerRecord(Func<string?, string> Transformer);
    public record PatternRecord(string Pattern, string? Name = null, List<TransformerRecord>? Transformers = null);
    
    private void AddEpisodePatterns() =>
        PtnPatterns["episode"] =
        [
            new(@"(?<![a-z])(?:e|ep)(?:\(?[0-9]{1,2}(?:-?(?:e|ep)?(?:[0-9]{1,2}))?\)?)(?![0-9])"),
            new(@"\ss?(\d{1,2})\s\-\s\d{1,2}\s"),
            new(@"\b[0-9]{1,2}x([0-9]{2})\b"),
            new($@"\bepisod(?:e|io){Extras.Delimiters}\d{1,2}\b"),
            new($"{Extras.LinkPatterns(PtnPatterns["season"].Skip(6).ToList())}{Extras.Delimiters}*P(?:ar)?t{Extras.Delimiters}*(\\d{{1,3}})"),
        ];

    private void AddSeasonPatterns() =>
        PtnPatterns["season"] =
        [
            new($@"\b(?:Seasons?){Extras.Delimiters}(\d{{1,2}})(?:(?:{Extras.Delimiters}|&|and|to){{1,3}}(\d{{1,2}})){{2,}}\b", ""),
            new(@"\ss?(\d{1,2})\s\-\s\d{1,2}\s", ""),
            new($@"\b{SeasonRangePattern}\b", ""),
            new(@"(?:s\d{1,2}[.+\s]*){2,}\b", ""),
            new($@"\b(?:Complete{Extras.Delimiters})?s([0-9]{{1,2}}){Extras.LinkPatterns(PtnPatterns["episode"])}?\b", ""),
            new(@"\b([0-9]{1,2})x[0-9]{2}\b", ""),
            new($@"[0-9]{{1,2}}(?:st|nd|rd|th){Extras.Delimiters}season", ""),
            new($@"Series{Extras.Delimiters}\d{{1,2}}", ""),
            new($@"\b(?:Complete{Extras.Delimiters})?Season[\. -][0-9]{{1,2}}\b", ""),
        ];
    
    private void AddDatePatterns()
    {
        PtnPatterns["year"] = [new(YearPattern, "")];
        PtnPatterns["month"] = [new($"(?:{YearPattern}){Extras.Delimiters}({MonthPattern}){Extras.Delimiters}(?:{DayPattern})", "")];
        PtnPatterns["day"] = [new($"(?:{YearPattern}){Extras.Delimiters}(?:{MonthPattern}){Extras.Delimiters}({DayPattern})", "")];
    }
    
    private void AddResolutionPatterns() =>
        PtnPatterns["resolution"] =
        [
            new(@"([0-9]{3,4}(?:p|i))", null, [new(Transformers.Lowercase)]),
            new($@"(8K|7680{Extras.Delimiters}?x{Extras.Delimiters}?4320p?)", "8K"),
            new($@"(5K|5120{Extras.Delimiters}?x{Extras.Delimiters}?2880p?)", "5K"),
            new($@"(4K UHD|UHD|3840{Extras.Delimiters}?x{Extras.Delimiters}?2160p?)", "2160p"),
            new($@"(4K|4096{Extras.Delimiters}?x{Extras.Delimiters}?2160p?)", "4K"),
            new($@"(QHD|QuadHD|WQHD|2560{Extras.Delimiters}?x{Extras.Delimiters}?1440p?)", "1440p"),
            new($@"(2K|2048{Extras.Delimiters}?x{Extras.Delimiters}?1080p?)", "2K"),
            new($@"(Full HD|FHD|1920{Extras.Delimiters}?x{Extras.Delimiters}?1080p?)", "1080p"),
            new($@"(HD|1280{Extras.Delimiters}?x{Extras.Delimiters}?720p?)", "720p"),
            new("(qHD)", "540p"),
            new("(SD)", "480p"),
        ];
    
    private void AddQualityPatterns() =>
        PtnPatterns["quality"] =
        [
            new(@"WEB[ -\.]?DL(?:Rip|Mux)?|HDRip", "WEB-DL"),
            new(@"WEB[ -]?Cap", "WEBCap"),
            new(@"W[EB]B[ -]?(?:Rip)|WEB", "WEBRip"),
            new(@"(?:HD)?CAM(?:-?Rip)?", "Cam"),
            new(@"(?:HD)?TS|TELESYNC|PDVD|PreDVDRip", "Telesync"),
            new(@"WP|WORKPRINT", "Workprint"),
            new(@"(?:HD)?TC|TELECINE", "Telecine"),
            new(@"(?:DVD)?SCR(?:EENER)?|BDSCR", "Screener"),
            new(@"DDC", "Digital Distribution Copy"),
            new(@"DVD-?(?:Rip|Mux)", "DVD-Rip"),
            new(@"DVDR|DVD-Full|Full-rip", "DVD-R"),
            new(@"PDTV|DVBRip", "PDTV"),
            new(@"DSR(?:ip)?|SATRip|DTHRip", "DSRip"),
            new(@"AHDTV(?:Mux)?", "AHDTV"),
            new(@"HDTV(?:Rip)?", "HDTV"),
            new(@"D?TVRip|DVBRip", "TVRip"),
            new(@"VODR(?:ip)?", "VODRip"),
            new(@"HD-Rip", "HD-Rip"),
            new($@"Blu-?Ray{Extras.Delimiters}Rip|BDR(?:ip)?", "BDRip"),
            new(@"Blu-?Ray|(?:US|JP)?BD(?:remux)?", "Blu-ray"),
            new(@"BR-?Rip", "BRRip"),
            new(@"HDDVD", "HD DVD"),
            new(@"PPV(?:Rip)?", "Pay-Per-View Rip"),
        ];
    
    private void AddNetworkPatterns()
    {
        var qualityPatterns = PtnPatterns["quality"];
        var linkedQualityPatterns = Extras.LinkPatterns(qualityPatterns);
        var networkPatterns = new List<PatternRecord>
        {
            new(@"ATVP", "Apple TV+"),
            new(@"AMZN|Amazon", "Amazon Studios"),
            new(@"NF|Netflix", "Netflix"),
            new(@"NICK", "Nickelodeon"),
            new(@"RED", "YouTube Premium"),
            new(@"DSNY?P", "Disney Plus"),
            new(@"DSNY", "DisneyNOW"),
            new(@"HMAX", "HBO Max"),
            new(@"HBO", "HBO"),
            new(@"HULU", "Hulu Networks"),
            new(@"MS?NBC", "MSNBC"),
            new(@"DCU", "DC Universe"),
            new(@"ID", "Investigation Discovery"),
            new(@"iT", "iTunes"),
            new(@"AS", "Adult Swim"),
            new(@"CRAV", "Crave"),
            new(@"CC", "Comedy Central"),
            new(@"SESO", "Seeso"),
            new(@"VRV", "VRV"),
            new(@"PCOK", "Peacock"),
            new(@"CBS", "CBS"),
            new(@"iP", "BBC iPlayer"),
            new(@"NBC", "NBC"),
            new(@"AMC", "AMC"),
            new(@"PBS", "PBS"),
            new(@"STAN", "Stan."),
            new(@"RTE", "RTE Player"),
            new(@"CR", "Crunchyroll"),
            new(@"ANPL", "Animal Planet Live"),
            new(@"DTV", "DirecTV Stream"),
            new(@"VICE", "VICE"),
        };

        var networkPatternsWithQuality = Extras.SuffixPatternWith([linkedQualityPatterns], networkPatterns, Extras.Delimiters);
        networkPatternsWithQuality.AddRange(new List<PatternRecord>
        {
            new("BBC", "BBC"),
            new("Hoichoi", "Hoichoi"),
            new("Zee5", "ZEE5"),
            new("Hallmark", "Hallmark"),
            new("Sony\\s?LIV", "SONY LIV"),
        });

        PtnPatterns["network"] = networkPatternsWithQuality;
    }
    
    private void AddCodecPatterns() =>
        PtnPatterns["codec"] =
        [
            new(@"xvid", "Xvid"),
            new(@"av1", "AV1"),
            new($@"[hx]{Extras.Delimiters}?264", "H.264"),
            new(@"AVC", "H.264"),
            new($@"HEVC(?:{Extras.Delimiters}Main{Extras.Delimiters}?10P?)", "H.265 Main 10"),
            new($@"[hx]{Extras.Delimiters}?265", "H.265"),
            new(@"HEVC", "H.265"),
            new($@"[h]{Extras.Delimiters}?263", "H.263"),
            new(@"VC-1", "VC-1"),
            new($@"MPEG{Extras.Delimiters}?2", "MPEG-2"),
        ];
    
    private void AddAudioPatterns()
    {
        var channelAudioOptions = Extras.GetChannelAudioOptions(
        [
            new("TrueHD", "Dolby TrueHD"),
            new("Atmos", "Dolby Atmos"),
            new($@"DD{Extras.Delimiters}?EX", "Dolby Digital EX"),
            new($@"DD|AC{Extras.Delimiters}?3|DolbyD", "Dolby Digital"),
            new($@"DDP|E{Extras.Delimiters}?AC{Extras.Delimiters}?3|EC{Extras.Delimiters}?3", "Dolby Digital Plus"),
            new($@"DTS{Extras.Delimiters}?HD(?:{Extras.Delimiters}?(?:MA|Masters?(?:{Extras.Delimiters}Audio)?))", "DTS-HD MA"),
            new("DTSMA", "DTS-HD MA"),
            new($@"DTS{Extras.Delimiters}?HD", "DTS-HD"),
            new($@"DTS{Extras.Delimiters}?ES", "DTS-ES"),
            new($@"DTS{Extras.Delimiters}?EX", "DTS-EX"),
            new($@"DTS{Extras.Delimiters}?X", "DTS:X"),
            new("DTS", "DTS"),
            new($@"HE{Extras.Delimiters}?AAC", "HE-AAC"),
            new($@"HE{Extras.Delimiters}?AACv2", "HE-AAC v2"),
            new($@"AAC{Extras.Delimiters}?LC", "AAC-LC"),
            new("AAC", "AAC"),
            new($@"Dual{Extras.Delimiters}Audios?", "Dual"),
            new($@"Custom{Extras.Delimiters}Audios?", "Custom"),
            new("FLAC", "FLAC"),
            new("OGG", "OGG"),
        ]);

        channelAudioOptions.AddRange(new List<PatternRecord>
        {
            new($@"7.1(?:{Extras.Delimiters}?ch(?:annel)?(?:{Extras.Delimiters}?Audio)?)?", "7.1"),
            new($@"5.1(?:{Extras.Delimiters}?ch(?:annel)?(?:{Extras.Delimiters}?Audio)?)?", "5.1"),
            new("MP3", null, [new(Transformers.Uppercase)]),
            new($@"2.0(?:{Extras.Delimiters}?ch(?:annel)?(?:{Extras.Delimiters}?Audio)?)?|2CH|stereo", "Dual"),
            new($@"1{Extras.Delimiters}?Ch(?:annel)?(?:{Extras.Delimiters}?Audio)?", "Mono"),
            new($@"(?:Original|Org){Extras.Delimiters}Aud(?:io)?", "Original"),
            new("LiNE", "LiNE"),
        });

        PtnPatterns["audio"] = channelAudioOptions;
    }
    
    private void AddRegionPatterns() =>
        PtnPatterns["region"] =
        [
            new(@"R[0-9]", null, [new(Transformers.Uppercase)]),
        ];
    
    private void AddExtendedPatterns() =>
        PtnPatterns["extended"] =
        [
            new("(EXTENDED(:?.CUT)?)", null, [new(Transformers.Uppercase)]),
        ];
    
    private void AddHardcodedPatterns() =>
        PtnPatterns["hardcoded"] =
        [
            new("HC", null, [new(Transformers.Uppercase)]),
        ];
    
    private void AddProperPatterns() =>
        PtnPatterns["proper"] =
        [
            new("PROPER", null, [new(Transformers.Uppercase)]),
        ];
    
    private void AddRepackPatterns() =>
        PtnPatterns["repack"] =
        [
            new("REPACK", null, [new(Transformers.Uppercase)]),
        ];
    
    private void AddFpsPatterns() =>
        PtnPatterns["fps"] =
        [
            new($@"([1-9][0-9]{{1,2}}){Extras.Delimiters}*fps", null, [new(Transformers.Lowercase)]),
        ];
    
    private void AddFiletypePatterns() =>
        PtnPatterns["filetype"] =
        [
            new(@"\.?(MKV|AVI|(?:SRT|SUB|SSA)$)", null, [new(Transformers.Uppercase)]),
            new("MP-?4", "MP4"),
            new(@"\.?(iso)$", "ISO"),
        ];
    
    private void AddWidescreenPatterns() =>
        PtnPatterns["widescreen"] =
        [
            new("WS", null, [new(Transformers.Uppercase)]),
        ];
    
    private void AddSitePatterns() =>
        PtnPatterns["site"] =
        [
            new(@"^(\[ ?([^\]]+?)\s?\])"),
            new(@"^((?:www\.)?[\w-]+\.[\w]{2,4})\s+-\s*"),
        ];
    
    private void AddLanguageAndSubtitlePatterns()
    {
        var langListPattern = $@"\b(?:{Extras.LinkPatterns(Extras.Langs)}(?:{Extras.Delimiters}+(?:dub(?:bed)?|{Extras.LinkPatterns(PtnPatterns["audio"])}))?{Extras.Delimiters}+|\b)";
        var subsListPattern = $@"(?:{Extras.LinkPatterns(Extras.Langs)}{Extras.Delimiters}*)";

        PtnPatterns["subtitles"] =
        [
            new($"sub(?:title|bed)?s?{Extras.Delimiters}*{subsListPattern}+"),
            new($"(?:soft{Extras.Delimiters}*)?{subsListPattern}+(?:(?:m(?:ulti(?:ple)?)?{Extras.Delimiters}*)?sub(?:title|bed)?s?)"),
            new("VOSTFR", "French"),
            new($"(?:m(?:ulti(?:ple)?)?{Extras.Delimiters}*)sub(?:title|bed)?s?"),
            new($"(?:m(?:ulti(?:ple)?)?[.\\s\\-+_\\/]*)?sub(?:title|bed)?s?{Extras.Delimiters}*"),
        ];

        PtnPatterns["language"] =
        [
            new($"({langListPattern}+)(?:{Extras.Delimiters}*{PtnPatterns["subtitles"][0].Pattern})"),
            new($"({langListPattern}+)(?!{Extras.Delimiters}*{Extras.LinkPatterns(PtnPatterns["subtitles"])})"),
            new($"({langListPattern}+)(?:{Extras.Delimiters}*{PtnPatterns["subtitles"][^2].Pattern})"),
        ];
    }
    
    private void AddSbsPatterns() =>
        PtnPatterns["sbs"] =
        [
            new("Half-SBS", "Half SBS"),
            new("SBS", null, [new(Transformers.Uppercase)]),
        ];
    
    private void AddUnratedPatterns() =>
        PtnPatterns["unrated"] =
        [
            new("UNRATED", null, [new(Transformers.Uppercase)]),
        ];
    
    private void AddSizePatterns() =>
        PtnPatterns["size"] =
        [
            new(@"\d+(?:\.\d+)?\s?(?:GB|MB)", null, [new(Transformers.Uppercase), new(Transformers.Replace(" "))]),
        ];
    
    private void AddBitDepthPatterns() =>
        PtnPatterns["bitDepth"] =
        [
            new("(8|10)-?bits?"),
        ];
    
    private void Add3DPatterns() =>
        PtnPatterns["3d"] =
        [
            new("3D"),
        ];

    private void AddInternalPatterns() =>
        PtnPatterns["internal"] =
        [
            new("iNTERNAL"),
        ];

    private void AddReadNfoPatterns() =>
        PtnPatterns["readnfo"] =
        [
            new("READNFO"),
        ];

    private void AddHDRPatterns() =>
        PtnPatterns["hdr"] =
        [
            new("HDR(?:10)?"),
        ];

    private void AddDocumentaryPatterns() =>
        PtnPatterns["documentary"] =
        [
            new("DOCU(?:menta?ry)?"),
        ];

    private void AddLimitedPatterns() =>
        PtnPatterns["limited"] =
        [
            new("LIMITED"),
        ];

    private void AddRemasteredPatterns() =>
        PtnPatterns["remastered"] =
        [
            new("REMASTERED"),
        ];

    private void AddDirectorsCutPatterns() =>
        PtnPatterns["directorsCut"] =
        [
            new("DC|Director'?s.?Cut"),
        ];

    private void AddUpscaledPatterns() =>
        PtnPatterns["upscaled"] =
        [
            new("(?:AI" + Extras.Delimiters + "*)?upscaled?"),
        ];
    
    private void AddUntouchedPatterns() =>
        PtnPatterns["untouched"] =
        [
            new("untouched"),
        ];

    private void AddRemuxPatterns() =>
        PtnPatterns["remux"] =
        [
            new("REMUX"),
        ];

    private void AddInternationalCutPatterns() =>
        PtnPatterns["internationalCut"] =
        [
            new("International" + Extras.Delimiters + "Cut"),
        ];

    private void AddGenrePatterns() =>
        PtnPatterns["genre"] =
        [
            new(@"\b\s*[\(\-\]]+\s*((?:" + Extras.LinkPatterns(Extras.Genres) + Extras.Delimiters + @"?)+)\b"),
        ];

    private static IReadOnlyCollection<string> PatternsOrdered =
    [
        "resolution",
        "quality",
        "season",
        "episode",
        "year",
        "month",
        "day",
        "codec",
        "audio",
        "region",
        "extended",
        "hardcoded",
        "proper",
        "repack",
        "filetype",
        "widescreen",
        "sbs",
        "site",
        "documentary",
        "language",
        "subtitles",
        "unrated",
        "size",
        "bitDepth",
        "3d",
        "internal",
        "readnfo",
        "network",
        "fps",
        "hdr",
        "limited",
        "remastered",
        "directorsCut",
        "upscaled",
        "untouched",
        "remux",
        "internationalCut",
        "genre",
    ];

    private static IReadOnlyCollection<string> PatternsAllowOverlap =
    [
        "season",
        "episode",
        "language",
        "subtitles",
        "sbs",
    ];
    
    private static Dictionary<string, string> SetupTypeMappings() =>
        new()
        {
            {"season", "integer"},
            {"episode", "integer"},
            {"bitDepth", "integer"},
            {"year", "integer"},
            {"month", "integer"},
            {"day", "integer"},
            {"fps", "integer"},
            {"extended", "boolean"},
            {"hardcoded", "boolean"},
            {"proper", "boolean"},
            {"repack", "boolean"},
            {"widescreen", "boolean"},
            {"unrated", "boolean"},
            {"3d", "boolean"},
            {"internal", "boolean"},
            {"readnfo", "boolean"},
            {"documentary", "boolean"},
            {"hdr", "boolean"},
            {"limited", "boolean"},
            {"remastered", "boolean"},
            {"directorsCut", "boolean"},
            {"upscaled", "boolean"},
            {"untouched", "boolean"},
            {"remux", "boolean"},
            {"internationalCut", "boolean"},
        };
}