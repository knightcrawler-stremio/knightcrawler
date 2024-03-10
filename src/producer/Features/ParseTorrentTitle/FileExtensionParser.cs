namespace Producer.Features.ParseTorrentTitle;

public static partial class FileExtensionParser
{
    [GeneratedRegex(@"\.[a-z0-9]{2,4}$", RegexOptions.IgnoreCase)]
    private static partial Regex FileExtensionExp();

    private static readonly List<string> _fileExtensions = new()
    {
        // Unknown
        ".webm",
        // SDTV
        ".m4v",
        ".3gp",
        ".nsv",
        ".ty",
        ".strm",
        ".rm",
        ".rmvb",
        ".m3u",
        ".ifo",
        ".mov",
        ".qt",
        ".divx",
        ".xvid",
        ".bivx",
        ".nrg",
        ".pva",
        ".wmv",
        ".asf",
        ".asx",
        ".ogm",
        ".ogv",
        ".m2v",
        ".avi",
        ".bin",
        ".dat",
        ".dvr-ms",
        ".mpg",
        ".mpeg",
        ".mp4",
        ".avc",
        ".vp3",
        ".svq3",
        ".nuv",
        ".viv",
        ".dv",
        ".fli",
        ".flv",
        ".wpl",

        // DVD
        ".img",
        ".iso",
        ".vob",

        // HD
        ".mkv",
        ".mk3d",
        ".ts",
        ".wtv",

        // Bluray
        ".m2ts",
    };

    public static string RemoveFileExtension(string title) =>
        FileExtensionExp().Replace(
            title, match =>
            {
                if (_fileExtensions.Any(ext => ext.Equals(match.Value, StringComparison.OrdinalIgnoreCase)))
                {
                    return string.Empty;
                }

                return match.Value;
            });
}