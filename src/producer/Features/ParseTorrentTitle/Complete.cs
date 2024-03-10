namespace Producer.Features.ParseTorrentTitle;

public static partial class Complete
{
    [GeneratedRegex(@"\b(NTSC|PAL)?.DVDR\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex CompleteDvdExp();
    
    [GeneratedRegex(@"\b(COMPLETE)\b", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex CompleteExp();
    public static bool? IsCompleteDvd(string title) => CompleteDvdExp().IsMatch(title) ? true : null;

    public static bool IsComplete(string title) => CompleteExp().IsMatch(title) || IsCompleteDvd(title) == true;
    
}