namespace Producer.Features.PTN;

public class DefaultHandlers(PTN parser)
{
    public void AddHandlers()
    {
        AddEpisodeCodeHandlers();
        AddResolutionHandlers();
        AddDateHandlers();
        AddYearHandlers();
        AddExtendedHandlers();
        AddConvertHandlers();
        AddHardcodedHandlers();
        AddProperHandlers();
        AddRepackHandlers();
        AddRetailHandlers();
        AddRemasteredHandlers();
        AddUnratedHandlers();
        AddRegionHandlers();
        AddSourceHandlers();
        AddVideoDepthHandlers();
        AddHdrHandlers();
        AddCodecHandlers();
        AddAudioHandlers();
        AddGroupHandlers();
        AddContainerHandlers();
        AddVolumesHandlers();
        AddSeasonHandlers();
        AddEpisodeHandlers();
        AddCompleteHandlers();
        AddDubbedHandlers();
    }

    private void AddEpisodeCodeHandlers()
    {
        parser.AddHandler("episodeCode", new Regex(@"\[[a-zA-Z0-9]{8}\](?=\.[a-zA-Z0-9]{1,5}$|$)"), Transformers.Uppercase, new() { Remove = true });
        parser.AddHandler("episodeCode", new Regex(@"\[[A-Z0-9]{8}\]"), Transformers.Uppercase, new() { Remove = true });
    }

    private void AddResolutionHandlers()
    {
        parser.AddHandler("resolution", new Regex(@"\b[([]?4k[)\]]?\b", RegexOptions.IgnoreCase), Transformers.Value("4k"), new() { Remove = true });
        parser.AddHandler("resolution", new Regex(@"21600?[pi]", RegexOptions.IgnoreCase), Transformers.Value("4k"), new() { SkipIfAlreadyFound = false, Remove = true });
        parser.AddHandler("resolution", new Regex(@"[([]?3840x\d{4}[)\]]?", RegexOptions.IgnoreCase), Transformers.Value("4k"), new() { Remove = true });
        parser.AddHandler("resolution", new Regex(@"[([]?1920x\d{3,4}[)\]]?", RegexOptions.IgnoreCase), Transformers.Value("1080p"), new() { Remove = true });
        parser.AddHandler("resolution", new Regex(@"[([]?1280x\d{3}[)\]]?", RegexOptions.IgnoreCase), Transformers.Value("720p"), new() { Remove = true });
        parser.AddHandler("resolution", new Regex(@"[([]?\d{3,4}x(\d{3,4})[)\]]?", RegexOptions.IgnoreCase), Transformers.Value("$1p"), new() { Remove = true });
        parser.AddHandler("resolution", new Regex(@"(480|720|1080)0[pi]", RegexOptions.IgnoreCase), Transformers.Value("$1p"), new() { Remove = true });
        parser.AddHandler("resolution", new Regex(@"(?:BD|HD|M)(720|1080|2160)"), Transformers.Value("$1p"), new() { Remove = true });
        parser.AddHandler("resolution", new Regex(@"(480|576|720|1080|2160)[pi]", RegexOptions.IgnoreCase), Transformers.Value("$1p"), new() { Remove = true });
        parser.AddHandler("resolution", new Regex(@"(?:^|\D)(\d{3,4})[pi]", RegexOptions.IgnoreCase), Transformers.Value("$1p"), new() { Remove = true });
    }

    private void AddDateHandlers()
    {
        parser.AddHandler("date", new Regex(@"(?<=\W|^)([([]?(?:19[6-9]|20[012])[0-9]([. \-/\\])(?:0[1-9]|1[012])\2(?:0[1-9]|[12][0-9]|3[01])[)\]]?)(?=\W|$)"), Transformers.Date("YYYY MM DD"), new() { Remove = true });
        parser.AddHandler("date", new Regex(@"(?<=\W|^)([([]?(?:0[1-9]|[12][0-9]|3[01])([. \-/\\])(?:0[1-9]|1[012])\2(?:19[6-9]|20[012])[0-9][)\]]?)(?=\W|$)"), Transformers.Date("DD MM YYYY"), new() { Remove = true });
        parser.AddHandler("date", new Regex(@"(?<=\W)([([]?(?:0[1-9]|1[012])([. \-/\\])(?:0[1-9]|[12][0-9]|3[01])\2(?:[0][1-9]|[0126789][0-9])[)\]]?)(?=\W|$)"), Transformers.Date("MM DD YY"), new() { Remove = true });
        parser.AddHandler("date", new Regex(@"(?<=\W)([([]?(?:0[1-9]|[12][0-9]|3[01])([. \-/\\])(?:0[1-9]|1[012])\2(?:[0][1-9]|[0126789][0-9])[)\]]?)(?=\W|$)"), Transformers.Date("DD MM YY"), new() { Remove = true });
        parser.AddHandler("date", new Regex(@"(?<=\W|^)([([]?(?:0?[1-9]|[12][0-9]|3[01])[. ]?(?:st|nd|rd|th)?([. \-/\\])(?:feb(?:ruary)?|jan(?:uary)?|mar(?:ch)?|apr(?:il)?|may|june?|july?|aug(?:ust)?|sept?(?:ember)?|oct(?:ober)?|nov(?:ember)?|dec(?:ember)?)\2(?:19[7-9]|20[012])[0-9][)\]]?)(?=\W|$)", RegexOptions.IgnoreCase), Transformers.Date("DD MMM YYYY"), new() { Remove = true });
        parser.AddHandler("date", new Regex(@"(?<=\W|^)([([]?(?:0?[1-9]|[12][0-9]|3[01])[. ]?(?:st|nd|rd|th)?([. \-/\\])(?:feb(?:ruary)?|jan(?:uary)?|mar(?:ch)?|apr(?:il)?|may|june?|july?|aug(?:ust)?|sept?(?:ember)?|oct(?:ober)?|nov(?:ember)?|dec(?:ember)?)\2(?:0[1-9]|[0126789][0-9])[)\]]?)(?=\W|$)", RegexOptions.IgnoreCase), Transformers.Date("DD MMM YY"), new() { Remove = true });
        parser.AddHandler("date", new Regex(@"(?<=\W|^)([([]?20[012][0-9](?:0[1-9]|1[012])(?:0[1-9]|[12][0-9]|3[01])[)\]]?)(?=\W|$)"), Transformers.Date("YYYYMMDD"), new() { Remove = true });
    }

    private void AddYearHandlers()
    {
        parser.AddHandler("year", new Regex(@"[([]?[ .]?((?:19\d|20[012])\d[ .]?-[ .]?(?:19\d|20[012])\d)[ .]?[)\]]?"), Transformers.YearRange, new() { Remove = true });
        parser.AddHandler("year", new Regex(@"[([][ .]?((?:19\d|20[012])\d[ .]?-[ .]?\d{2})[ .]?[)\]]"), Transformers.YearRange, new() { Remove = true });
        parser.AddHandler("year", new Regex(@"[([]?(?!^)(?<!\d|Cap[. ]?)((?:19\d|20[012])\d)(?!\d|kbps)[)\]]?", RegexOptions.IgnoreCase), Transformers.Integer, new() { Remove = true });
        parser.AddHandler("year", new Regex(@"^[([]?((?:19\d|20[012])\d)(?!\d|kbps)[)\]]?", RegexOptions.IgnoreCase), Transformers.Integer, new() { Remove = true });
    }

    private void AddExtendedHandlers() => parser.AddHandler("extended", new Regex(@"EXTENDED"), Transformers.Boolean, Options.DefaultOptions);

    private void AddConvertHandlers() => parser.AddHandler("convert", new Regex(@"CONVERT"), Transformers.Boolean, Options.DefaultOptions);

    private void AddHardcodedHandlers() => parser.AddHandler("hardcoded", new Regex(@"HC|HARDCODED"), Transformers.Boolean, Options.DefaultOptions);

    private void AddProperHandlers() => parser.AddHandler("proper", new Regex(@"(?:REAL.)?PROPER"), Transformers.Boolean, Options.DefaultOptions);

    private void AddRepackHandlers() => parser.AddHandler("proper", new Regex(@"(?:REAL.)?PROPER"), Transformers.Boolean, Options.DefaultOptions);

    private void AddRetailHandlers() => parser.AddHandler("retail", new Regex(@"\bRetail\b", RegexOptions.IgnoreCase), Transformers.Boolean, Options.DefaultOptions);

    private void AddRemasteredHandlers() => parser.AddHandler("remastered", new Regex(@"\bRemaster(?:ed)?\b", RegexOptions.IgnoreCase), Transformers.Boolean, Options.DefaultOptions);

    private void AddUnratedHandlers() => parser.AddHandler("unrated", new Regex(@"\bunrated|uncensored\b", RegexOptions.IgnoreCase), Transformers.Boolean, Options.DefaultOptions);

    private void AddRegionHandlers() => parser.AddHandler("region", new Regex(@"R\d\b"), Transformers.None, new() { SkipIfFirst = true });

    private void AddSourceHandlers()
    {
        parser.AddHandler("source", new Regex(@"\b(?:H[DQ][ .-]*)?CAM(?:H[DQ])?(?:[ .-]*Rip)?\b", RegexOptions.IgnoreCase), Transformers.Value("CAM"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\b(?:H[DQ][ .-]*)?S[ .-]*print", RegexOptions.IgnoreCase), Transformers.Value("CAM"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\b(?:HD[ .-]*)?T(?:ELE)?S(?:YNC)?(?:Rip)?\b", RegexOptions.IgnoreCase), Transformers.Value("TeleSync"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\b(?:HD[ .-]*)?T(?:ELE)?C(?:INE)?(?:Rip)?\b"), Transformers.Value("TeleCine"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bBlu[ .-]*Ray\b(?=.*remux)", RegexOptions.IgnoreCase), Transformers.Value("BluRay REMUX"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"(?:BD|BR|UHD)[- ]?remux", RegexOptions.IgnoreCase), Transformers.Value("BluRay REMUX"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"(?<=remux.*)\bBlu[ .-]*Ray\b", RegexOptions.IgnoreCase), Transformers.Value("BluRay REMUX"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bBlu[ .-]*Ray\b(?![ .-]*Rip)", RegexOptions.IgnoreCase), Transformers.Value("BluRay"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bUHD[ .-]*Rip\b", RegexOptions.IgnoreCase), Transformers.Value("UHDRip"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bHD[ .-]*Rip\b", RegexOptions.IgnoreCase), Transformers.Value("HDRip"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bMicro[ .-]*HD\b", RegexOptions.IgnoreCase), Transformers.Value("HDRip"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\b(?:BR|Blu[ .-]*Ray)[ .-]*Rip\b", RegexOptions.IgnoreCase), Transformers.Value("BRRip"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bBD[ .-]*Rip\b|\bBDR\b|\bBD-RM\b|[[(]BD[\]) .,-]", RegexOptions.IgnoreCase), Transformers.Value("BDRip"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\b(?:HD[ .-]*)?DVD[ .-]*Rip\b", RegexOptions.IgnoreCase), Transformers.Value("DVDRip"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bVHS[ .-]*Rip\b", RegexOptions.IgnoreCase), Transformers.Value("DVDRip"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\b(?:DVD?|BD|BR)?[ .-]*Scr(?:eener)?\b", RegexOptions.IgnoreCase), Transformers.Value("SCR"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bP(?:re)?DVD(?:Rip)?\b", RegexOptions.IgnoreCase), Transformers.Value("SCR"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bDVD(?:R\d?)?\b", RegexOptions.IgnoreCase), Transformers.Value("DVD"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bVHS\b", RegexOptions.IgnoreCase), Transformers.Value("DVD"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bPPVRip\b", RegexOptions.IgnoreCase), Transformers.Value("PPVRip"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bHD[ .-]*TV(?:Rip)?\b", RegexOptions.IgnoreCase), Transformers.Value("HDTV"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bDVB[ .-]*(?:Rip)?\b", RegexOptions.IgnoreCase), Transformers.Value("HDTV"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bSAT[ .-]*Rips?\b", RegexOptions.IgnoreCase), Transformers.Value("SATRip"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bTVRips?\b", RegexOptions.IgnoreCase), Transformers.Value("TVRip"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bR5\b", RegexOptions.IgnoreCase), Transformers.Value("R5"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bWEB[ .-]*DL(?:Rip)?\b", RegexOptions.IgnoreCase), Transformers.Value("WEB-DL"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\bWEB[ .-]*Rip\b", RegexOptions.IgnoreCase), Transformers.Value("WEBRip"), new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\b(?:DL|WEB|BD|BR)MUX\b", RegexOptions.IgnoreCase), Transformers.None, new() { Remove = true });
        parser.AddHandler("source", new Regex(@"\b(DivX|XviD)\b"), Transformers.None, new() { Remove = true });
    }

    private void AddVideoDepthHandlers()
    {
        parser.AddHandler("bitDepth", new Regex(@"(?:8|10|12)[- ]?bit", RegexOptions.IgnoreCase), Transformers.Lowercase, new() { Remove = true });
        parser.AddHandler("bitDepth", new Regex(@"\bhevc\s?10\b", RegexOptions.IgnoreCase), Transformers.Value("10bit"), Options.DefaultOptions);
        parser.AddHandler("bitDepth", new Regex(@"\bhdr10\b", RegexOptions.IgnoreCase), Transformers.Value("10bit"), Options.DefaultOptions);
        parser.AddHandler("bitDepth", new Regex(@"\bhi10\b", RegexOptions.IgnoreCase), Transformers.Value("10bit"), Options.DefaultOptions);
        parser.AddHandler("bitDepth", Handler.CreateHandlerFromDelegate("bitDepth", CustomBitDepthHandlerOne()), Transformers.None, Options.DefaultOptions);
    }

    private void AddHdrHandlers()
    {
        parser.AddHandler("hdr", new Regex(@"\bDV\b|dolby.?vision|\bDoVi\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("DV")), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler("hdr", new Regex(@"HDR10(?:\+|plus)", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("HDR10+")), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler("hdr", new Regex(@"\bHDR(?:10)?\b", RegexOptions.IgnoreCase), Transformers.UniqConcat(Transformers.Value("HDR")), new() { Remove = true, SkipIfAlreadyFound = false });
    }

    private void AddCodecHandlers()
    {
        parser.AddHandler("codec", new Regex(@"\b[xh][-. ]?26[45]", RegexOptions.IgnoreCase), Transformers.Lowercase, new() { Remove = true });
        parser.AddHandler("codec", new Regex(@"\bhevc(?:\s?10)?\b", RegexOptions.IgnoreCase), Transformers.Value("hevc"), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler("codec", new Regex(@"\b(?:dvix|mpeg2|divx|xvid|avc)\b", RegexOptions.IgnoreCase), Transformers.Lowercase, new() { Remove = true, SkipIfAlreadyFound = false });
    }

    private void AddAudioHandlers()
    {
        parser.AddHandler("audio", new Regex(@"7\.1[. ]?Atmos\b", RegexOptions.IgnoreCase), Transformers.Value("7.1 Atmos"), new() { Remove = true });
        parser.AddHandler("audio", new Regex(@"\b(?:mp3|Atmos|DTS(?:-HD)?|TrueHD)\b", RegexOptions.IgnoreCase), Transformers.Lowercase, Options.DefaultOptions);
        parser.AddHandler("audio", new Regex(@"\bFLAC(?:\+?2\.0)?(?:x[2-4])?\b", RegexOptions.IgnoreCase), Transformers.Value("flac"), new() { Remove = true });
        parser.AddHandler("audio", new Regex(@"\bEAC-?3(?:[. -]?[256]\.[01])?", RegexOptions.IgnoreCase), Transformers.Value("eac3"), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler("audio", new Regex(@"\bAC-?3(?:[.-]5\.1|x2\.?0?)?\b", RegexOptions.IgnoreCase), Transformers.Value("ac3"), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler("audio", new Regex(@"\b5\.1(?:x[2-4]+)?\+2\.0(?:x[2-4])?\b", RegexOptions.IgnoreCase), Transformers.Value("2.0"), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler("audio", new Regex(@"\b2\.0(?:x[2-4]|\+5\.1(?:x[2-4])?)\b", RegexOptions.IgnoreCase), Transformers.Value("2.0"), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler("audio", new Regex(@"\b5\.1ch\b", RegexOptions.IgnoreCase), Transformers.Value("ac3"), new() { Remove = true, SkipIfAlreadyFound = false });
        parser.AddHandler("audio", new Regex(@"\bDD5[. ]?1\b", RegexOptions.IgnoreCase), Transformers.Value("dd5.1"), new() { Remove = true });
        parser.AddHandler("audio", new Regex(@"\bQ?AAC(?:[. ]?2[. ]0|x2)?\b", RegexOptions.IgnoreCase), Transformers.Value("aac"), new() { Remove = true });
    }

    private void AddGroupHandlers() => parser.AddHandler("group", new Regex(@"- ?(?!\d+$|S\d+|\d+x|ep?\d+|[^[]+]$)([^\-. []+[^\-. [)\]\d][^\-. [)\]]*)(?:\[[\w.-]+])?(?=\.\w{2,4}$|$)", RegexOptions.IgnoreCase), Transformers.None, new() { Remove = true });

    private void AddContainerHandlers() => parser.AddHandler("container", new Regex(@"\.?[[(]?\b(MKV|AVI|MP4|WMV|MPG|MPEG)\b[\])]?", RegexOptions.IgnoreCase), Transformers.Lowercase, Options.DefaultOptions);

    private void AddVolumesHandlers()
    {
        parser.AddHandler("volumes", new Regex(@"\bvol(?:s|umes?)?[. -]*(?:\d{1,2}[., +/\\&-]+)+\d{1,2}\b", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler("volumes", Handler.CreateHandlerFromDelegate("volumes", CustomVolumesHandlerOne()), Transformers.Range, new() { Remove = true });
    }

    private void AddSeasonHandlers()
    {
        parser.AddHandler("seasons", new Regex(@"(?:complete\W|seasons?\W|\W|^)((?:s\d{1,2}[., +/\\&-]+)+s\d{1,2}\b)", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler("seasons", new Regex(@"(?:complete\W|seasons?\W|\W|^)[([]?(s\d{2,}-\d{2,}\b)[)\]]?", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler("seasons", new Regex(@"(?:complete\W|seasons?\W|\W|^)[([]?(s[1-9]-[2-9]\b)[)\]]?", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler("seasons", new Regex(@"(?:(?:\bthe\W)?\bcomplete\W)?(?:seasons?|[Сс]езони?|temporadas?)[. ]?[-:]?[. ]?[([]?((?:\d{1,2}[., /\\&]+)+\d{1,2}\b)[)\]]?", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler("seasons", new Regex(@"(?:(?:\bthe\W)?\bcomplete\W)?(?:seasons|[Сс]езони?|temporadas?)[. ]?[-:]?[. ]?[([]?((?:\d{1,2}[. -]+)+[1-9]\d?\b)[)\]]?", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler("seasons", new Regex(@"(?:(?:\bthe\W)?\bcomplete\W)?season[. ]?[([]?((?:\d{1,2}[. -]+)+[1-9]\d?\b)[)\]]?(?!.*\.\w{2,4}$)", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler("seasons", new Regex(@"(?:(?:\bthe\W)?\bcomplete\W)?\bseasons?\b[. -]?(\d{1,2}[. -]?(?:to|thru|and|\+|:)[. -]?\d{1,2})\b", RegexOptions.IgnoreCase), Transformers.Range, new() { Remove = true });
        parser.AddHandler("seasons", new Regex(@"(?:(?:\bthe\W)?\bcomplete\W)?(?:saison|seizoen|season|series|temp(?:orada)?):?[. ]?(\d{1,2})", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("seasons", new Regex(@"(\d{1,2})(?:-?й)?[. _]?(?:[Сс]езон|sez(?:on)?)(?:\W?\D|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("seasons", new Regex(@"[Сс]езон:?[. _]?№?(\d{1,2})(?!\d)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("seasons", new Regex(@"(?:\D|^)(\d{1,2})Â?[°ºªa]?[. ]*temporada", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), new() { Remove = true });
        parser.AddHandler("seasons", new Regex(@"t(\d{1,3})(?:[ex]+|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), new() { Remove = true });
        parser.AddHandler("seasons", new Regex(@"(?:(?:\bthe\W)?\bcomplete)?(?:\W|^)s(\d{1,3})(?:[\Wex]|\d{2}\b|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), new() { SkipIfAlreadyFound = false });
        parser.AddHandler("seasons", new Regex(@"(?:(?:\bthe\W)?\bcomplete)?(?:\W|^)(\d{1,2})[. ]?(?:st|nd|rd|th)[. ]*season", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("seasons", new Regex(@"(?:\D|^)(\d{1,2})[xх]\d{1,3}(?:\D|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("seasons", new Regex(@"\bSn([1-9])(?:\D|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("seasons", new Regex(@"[[(](\d{1,2})\.\d{1,3}[)\]]", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("seasons", new Regex(@"-\s?(\d{1,2})\.\d{2,3}\s?-", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("seasons", new Regex(@"(?:^|\/)(\d{1,2})-\d{2}\b(?!-\d)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("seasons", new Regex(@"[^\w-](\d{1,2})-\d{2}(?=\.\w{2,4}$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("seasons", new Regex(@"(?<!\bEp?(?:isode)? ?\d+\b.*)\b(\d{2})[ ._]\d{2}(?:.F)?\.\w{2,4}$", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("seasons", new Regex(@"\bEp(?:isode)?\W+(\d{1,2})\.\d{1,3}\b", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("season", Handler.CreateHandlerFromDelegate("season", CustomSeasonHandlerOne()), Transformers.None, Options.DefaultOptions);
    }

    private void AddEpisodeHandlers()
    {
        parser.AddHandler("episodes", new Regex(@"(?:[\W\d]|^)e[ .]?[([]?(\d{1,3}(?:[ .-]*(?:[&+]|e){1,2}[ .]?\d{1,3})+)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?:[\W\d]|^)ep[ .]?[([]?(\d{1,3}(?:[ .-]*(?:[&+]|ep){1,2}[ .]?\d{1,3})+)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?:[\W\d]|^)\d+[xх][ .]?[([]?(\d{1,3}(?:[ .]?[xх][ .]?\d{1,3})+)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?:[\W\d]|^)(?:episodes?|[Сс]ерии:?)[ .]?[([]?(\d{1,3}(?:[ .+]*[&+][ .]?\d{1,3})+)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"[([]?(?:\D|^)(\d{1,3}[ .]?ao[ .]?\d{1,3})[)\]]?(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?:[\W\d]|^)(?:e|eps?|episodes?|[Сс]ерии:?|\d+[xх])[ .]*[([]?(\d{1,3}(?:-\d{1,3})+)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?:\W|^)[st]\d{1,2}[. ]?[xх-]?[. ]?(?:e|x|х|ep|-|\.)[. ]?(\d{1,3})(?:[abc]|v0?[1-4]|\D|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"\b[st]\d{2}(\d{2})\b", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?:\W|^)(\d{1,3}(?:[ .]*~[ .]*\d{1,3})+)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"-\s(\d{1,3}[ .]*-[ .]*\d{1,3})(?!-\d)(?:\W|$)", RegexOptions.IgnoreCase), Transformers.Range, Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"s\d{1,2}\s?\((\d{1,3}[ .]*-[ .]*\d{1,3})\)", RegexOptions.IgnoreCase), Transformers.Range, Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?:^|\/)\d{1,2}-(\d{2})\b(?!-\d)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?<!\d-)\b\d{1,2}-(\d{2})(?=\.\w{2,4}$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?<!(?:seasons?|[Сс]езони?)\W*)(?:[ .([-]|^)(\d{1,3}(?:[ .]?[,&+~][ .]?\d{1,3})+)(?:[ .)\]-]|$)", RegexOptions.IgnoreCase), Transformers.Range, Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?<!(?:seasons?|[Сс]езони?)\W*)(?:[ .([-]|^)(\d{1,3}(?:-\d{1,3})+)(?:[ .)(\]]|-\D|$)", RegexOptions.IgnoreCase), Transformers.Range, Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"\bEp(?:isode)?\W+\d{1,2}\.(\d{1,3})\b", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?:\b[ée]p?(?:isode)?|[Ээ]пизод|[Сс]ер(?:ии|ия|\.)?|cap(?:itulo)?|epis[oó]dio)[. ]?[-:#№]?[. ]?(\d{1,4})(?:[abc]|v0?[1-4]|\W|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"\b(\d{1,3})(?:-?я)?[ ._-]*(?:ser(?:i?[iyj]a|\b)|[Сс]ер(?:ии|ия|\.)?)/i"), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?:\D|^)\d{1,2}[. ]?[xх][. ]?(\d{1,3})(?:[abc]|v0?[1-4]|\D|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"[[(]\d{1,2}\.(\d{1,3})[)\]]", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"\b[Ss]\d{1,2}[ .](\d{1,2})\b", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"-\s?\d{1,2}\.(\d{2,3})\s?-/", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?<=\D|^)(\d{1,3})[. ]?(?:of|из|iz)[. ]?\d{1,3}(?=\D|$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"\b\d{2}[ ._-](\d{2})(?:.F)?\.\w{2,4}$", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", new Regex(@"(?<!^)\[(\d{2,3})](?!(?:\.\w{2,4})?$)", RegexOptions.IgnoreCase), Transformers.Array(Transformers.Integer), Options.DefaultOptions);
        parser.AddHandler("episodes", CustomEpisodeHandlerOne(), Transformers.None, Options.DefaultOptions);
        parser.AddHandler("episodes", CustomEpisodeHandlerTwo(), Transformers.None, Options.DefaultOptions);
    }

    private void AddCompleteHandlers()
    {
        parser.AddHandler("complete", new Regex(@"(?:\bthe\W)?(?:\bcomplete|collection|dvd)?\b[ .]?\bbox[ .-]?set\b", RegexOptions.IgnoreCase), Transformers.Boolean, Options.DefaultOptions);
        parser.AddHandler("complete", new Regex(@"(?:\bthe\W)?(?:\bcomplete|collection|dvd)?\b[ .]?\bmini[ .-]?series\b", RegexOptions.IgnoreCase), Transformers.Boolean, Options.DefaultOptions);
        parser.AddHandler("complete", new Regex(@"(?:\bthe\W)?(?:\bcomplete|full|all)\b.*\b(?:series|seasons|collection|episodes|set|pack|movies)\b", RegexOptions.IgnoreCase), Transformers.Boolean, Options.DefaultOptions);
        parser.AddHandler("complete", new Regex(@"\b(?:series|seasons|movies?)\b.*\b(?:complete|collection)\b", RegexOptions.IgnoreCase), Transformers.Boolean, Options.DefaultOptions);
        parser.AddHandler("complete", new Regex(@"(?:\bthe\W)?\bultimate\b[ .]\bcollection\b", RegexOptions.IgnoreCase), Transformers.Boolean, new() { SkipIfAlreadyFound = false });
        parser.AddHandler("complete", new Regex(@"\bcollection\b.*\b(?:set|pack|movies)\b", RegexOptions.IgnoreCase), Transformers.Boolean, Options.DefaultOptions);
        parser.AddHandler("complete", new Regex(@"\bcollection\b", RegexOptions.IgnoreCase), Transformers.Boolean, new() { SkipFromTitle = true });
        parser.AddHandler("complete", new Regex(@"duology|trilogy|quadr[oi]logy|tetralogy|pentalogy|hexalogy|heptalogy|anthology|saga", RegexOptions.IgnoreCase), Transformers.Boolean, new() { SkipIfAlreadyFound = false });
    }

    private void AddDubbedHandlers()
    {
        parser.AddHandler("dubbed", new Regex(@"\b(?:DUBBED|dublado|dubbing|DUBS?)\b", RegexOptions.IgnoreCase), Transformers.Boolean, Options.DefaultOptions);
        parser.AddHandler("dubbed", CustomDubbedHandlerOne(), Transformers.None, Options.DefaultOptions);
    }

    private static Func<Dictionary<string, object>, HandlerResult> CustomBitDepthHandlerOne()
    {
        return BitDepthFunc;

        HandlerResult BitDepthFunc(Dictionary<string, object> context)
        {
            var result = context["result"] as Dictionary<string, object>;

            if (result.ContainsKey("bitDepth"))
            {
                var bitDepth = result["bitDepth"] as string;
                result["bitDepth"] = bitDepth.Replace(" ", "", StringComparison.OrdinalIgnoreCase).Replace("-", "", StringComparison.OrdinalIgnoreCase);
            }

            return null;
        }
    }

    private static Func<Dictionary<string, object>, HandlerResult> CustomVolumesHandlerOne()
    {
        return VolumesFunc;

        HandlerResult VolumesFunc(Dictionary<string, object> context)
        {
            var title = context["title"] as string;
            var result = context["result"] as Dictionary<string, object>;
            var matched = context["matched"] as Dictionary<string, object>;

            var startIndex = matched.ContainsKey("year") && matched["year"] is Match yearMatch ? yearMatch.Index : 0;
            var match = Regex.Match(title[startIndex..], @"\bvol(?:ume)?[. -]*(\d{1,2})", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                matched["volumes"] = new
                {
                    match = match.Value, matchIndex = match.Index
                };
                result["volumes"] = Transformers.Array(Transformers.Integer)(match.Value, match.Groups[1].Value);
                return new()
                {
                    RawMatch = match.Value, MatchIndex = match.Index, Remove = true
                };
            }

            return null;
        }
    }

    private static Func<Dictionary<string, object>, HandlerResult> CustomSeasonHandlerOne()
    {
        return SeasonFunc;

        HandlerResult SeasonFunc(Dictionary<string, object> context)
        {
            var result = context["result"] as Dictionary<string, object>;

            if (result.ContainsKey("seasons"))
            {
                var seasons = result["seasons"] as List<int>;

                if (seasons != null && seasons.Count == 1)
                {
                    result["season"] = seasons[0];
                }
            }

            return null;
        }
    }

    private static Func<Dictionary<string, object>, HandlerResult> CustomEpisodeHandlerOne()
    {
        return EpisodeFunc;

        HandlerResult EpisodeFunc(Dictionary<string, object> context)
        {
            var title = context["title"] as string;
            var result = context["result"] as Dictionary<string, object>;
            var matched = context["matched"] as Dictionary<string, object>;

            if (!result.ContainsKey("episodes"))
            {
                var startIndexes = new List<int> { matched.ContainsKey("year") ? (int)matched["year"] : 0, matched.ContainsKey("seasons") ? (int)matched["seasons"] : 0 };
                var endIndexes = new List<int> { matched.ContainsKey("resolution") ? (int)matched["resolution"] : title.Length, matched.ContainsKey("source") ? (int)matched["source"] : title.Length, matched.ContainsKey("codec") ? (int)matched["codec"] : title.Length, matched.ContainsKey("audio") ? (int)matched["audio"] : title.Length };
                var startIndex = startIndexes.Min();
                var endIndex = endIndexes.Min();
                var beginningTitle = title.Substring(0, endIndex);
                var middleTitle = title.Substring(startIndex, endIndex - startIndex);

                var matches = Regex.Match(beginningTitle, @"(?<!movie\W*|film\W*|^)(?:[ .]+-[ .]+|[([][ .]*)(\d{1,4})(?:a|b|v\d)?(?:\W|$)(?!movie|film)", RegexOptions.IgnoreCase) ??
                              Regex.Match(middleTitle, @"^(?:[([-][ .]?)?(\d{1,4})(?:a|b|v\d)?(?:\W|$)(?!movie|film)", RegexOptions.IgnoreCase);

                if (matches.Success)
                {
                    result["episodes"] = new List<int> { int.Parse(matches.Groups[matches.Groups.Count - 1].Value) };
                    return new HandlerResult { MatchIndex = title.IndexOf(matches.Value, StringComparison.OrdinalIgnoreCase) };
                }
            }

            return null;
        }
    }

    private static Func<Dictionary<string, object>, HandlerResult> CustomEpisodeHandlerTwo()
    {
        return EpisodeFunc;

        HandlerResult EpisodeFunc(Dictionary<string, object> context)
        {
            var result = context["result"] as Dictionary<string, object>;

            if (result.ContainsKey("episodes") && ((List<int>)result["episodes"]).Count == 1)
            {
                result["episode"] = ((List<int>)result["episodes"])[0];
            }

            return null;
        }
    }

    private static Func<Dictionary<string, object>, HandlerResult> CustomDubbedHandlerOne()
    {
        return DubbedHandler;

        HandlerResult DubbedHandler(Dictionary<string, object> context)
        {
            if (context.TryGetValue("result", out var resultObj) && resultObj is Dictionary<string, object> result)
            {
                if (result.TryGetValue("languages", out var languagesObj) && languagesObj is List<string> languages)
                {
                    if (new[] { "multi audio", "dual audio" }.Any(l => languages.Contains(l)))
                    {
                        result["dubbed"] = true;
                    }
                }
            }
            return new HandlerResult { MatchIndex = 0 };
        }
    }
}
