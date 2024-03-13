namespace Producer.Features.PTN.Handlers;

public class SourceHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Source, new Regex(@"\b(?:H[DQ][ .-]*)?CAM(?:H[DQ])?(?:[ .-]*Rip)?\b", RegexOptions.IgnoreCase), Transformers.Value("CAM"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\b(?:H[DQ][ .-]*)?S[ .-]*print", RegexOptions.IgnoreCase), Transformers.Value("CAM"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\b(?:HD[ .-]*)?T(?:ELE)?S(?:YNC)?(?:Rip)?\b", RegexOptions.IgnoreCase), Transformers.Value("TeleSync"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\b(?:HD[ .-]*)?T(?:ELE)?C(?:INE)?(?:Rip)?\b"), Transformers.Value("TeleCine"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bBlu[ .-]*Ray\b(?=.*remux)", RegexOptions.IgnoreCase), Transformers.Value("BluRay REMUX"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"(?:BD|BR|UHD)[- ]?remux", RegexOptions.IgnoreCase), Transformers.Value("BluRay REMUX"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"(?<=remux.*)\bBlu[ .-]*Ray\b", RegexOptions.IgnoreCase), Transformers.Value("BluRay REMUX"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bBlu[ .-]*Ray\b(?![ .-]*Rip)", RegexOptions.IgnoreCase), Transformers.Value("BluRay"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bUHD[ .-]*Rip\b", RegexOptions.IgnoreCase), Transformers.Value("UHDRip"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bHD[ .-]*Rip\b", RegexOptions.IgnoreCase), Transformers.Value("HDRip"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bMicro[ .-]*HD\b", RegexOptions.IgnoreCase), Transformers.Value("HDRip"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\b(?:BR|Blu[ .-]*Ray)[ .-]*Rip\b", RegexOptions.IgnoreCase), Transformers.Value("BRRip"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bBD[ .-]*Rip\b|\bBDR\b|\bBD-RM\b|[[(]BD[\]) .,-]", RegexOptions.IgnoreCase), Transformers.Value("BDRip"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\b(?:HD[ .-]*)?DVD[ .-]*Rip\b", RegexOptions.IgnoreCase), Transformers.Value("DVDRip"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bVHS[ .-]*Rip\b", RegexOptions.IgnoreCase), Transformers.Value("DVDRip"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\b(?:DVD?|BD|BR)?[ .-]*Scr(?:eener)?\b", RegexOptions.IgnoreCase), Transformers.Value("SCR"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bP(?:re)?DVD(?:Rip)?\b", RegexOptions.IgnoreCase), Transformers.Value("SCR"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bDVD(?:R\d?)?\b", RegexOptions.IgnoreCase), Transformers.Value("DVD"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bVHS\b", RegexOptions.IgnoreCase), Transformers.Value("DVD"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bPPVRip\b", RegexOptions.IgnoreCase), Transformers.Value("PPVRip"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bHD[ .-]*TV(?:Rip)?\b", RegexOptions.IgnoreCase), Transformers.Value("HDTV"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bDVB[ .-]*(?:Rip)?\b", RegexOptions.IgnoreCase), Transformers.Value("HDTV"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bSAT[ .-]*Rips?\b", RegexOptions.IgnoreCase), Transformers.Value("SATRip"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bTVRips?\b", RegexOptions.IgnoreCase), Transformers.Value("TVRip"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bR5\b", RegexOptions.IgnoreCase), Transformers.Value("R5"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bWEB[ .-]*DL(?:Rip)?\b", RegexOptions.IgnoreCase), Transformers.Value("WEB-DL"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\bWEB[ .-]*Rip\b", RegexOptions.IgnoreCase), Transformers.Value("WEBRip"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\b(?:DL|WEB|BD|BR)MUX\b", RegexOptions.IgnoreCase), Transformers.None, new() { Remove = true });
        parser.AddHandler(ResultKeys.Source, new Regex(@"\b(DivX|XviD)\b"), Transformers.None, new() { Remove = true });
    }
}