namespace Producer.Features.PTN;

public class ParseTorrentNameOptions
{
    public bool? SkipIfAlreadyFound { get; set; }
    public bool? SkipFromTitle { get; set; }
    public bool? SkipIfFirst { get; set; }
    public bool? Remove { get; set; }
    public string? Value { get; set; }

    public static ParseTorrentNameOptions ExtendOptions(ParseTorrentNameOptions? options)
    {
        options ??= new();

        var defaultOptions = DefaultOptions;

        options.SkipIfAlreadyFound ??= defaultOptions.SkipIfAlreadyFound;
        options.SkipFromTitle ??= defaultOptions.SkipFromTitle;
        options.SkipIfFirst ??= defaultOptions.SkipIfFirst;
        options.Remove ??= defaultOptions.Remove;

        return options;
    }

    public static ParseTorrentNameOptions DefaultOptions =>
        new()
        {
            SkipIfAlreadyFound = true,
            SkipFromTitle = false,
            SkipIfFirst = false,
            Remove = false,
        };
}
