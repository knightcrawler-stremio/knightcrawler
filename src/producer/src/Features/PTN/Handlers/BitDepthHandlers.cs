namespace Producer.Features.PTN.Handlers;

public class BitDepthHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.BitDepth, new Regex(@"(?:8|10|12)[- ]?bit", RegexOptions.IgnoreCase), Transformers.Lowercase, new() {Remove = true});
        parser.AddHandler(ResultKeys.BitDepth, new Regex(@"\bhevc\s?10\b", RegexOptions.IgnoreCase), Transformers.Value("10bit"), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.BitDepth, new Regex(@"\bhdr10\b", RegexOptions.IgnoreCase), Transformers.Value("10bit"), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.BitDepth, new Regex(@"\bhi10\b", RegexOptions.IgnoreCase), Transformers.Value("10bit"), ParseTorrentNameOptions.DefaultOptions);
        parser.AddHandler(ResultKeys.BitDepth, Handler.CreateHandlerFromDelegate(ResultKeys.BitDepth, CustomBitDepthHandlerOne), Transformers.None, ParseTorrentNameOptions.DefaultOptions);
    }

    private static Func<Dictionary<string, object>, HandlerResult> CustomBitDepthHandlerOne
        => context =>
        {
            var result = context[ResultKeys.Result] as Dictionary<string, object>;

            if (result.TryGetValue(ResultKeys.BitDepth, out var value))
            {
                var bitDepth = value as string;
                result[ResultKeys.BitDepth] = bitDepth.Replace(" ", "", StringComparison.OrdinalIgnoreCase).Replace("-", "", StringComparison.OrdinalIgnoreCase);
            }

            return null;
        };
}