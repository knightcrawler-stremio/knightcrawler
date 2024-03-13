namespace Producer.Features.PTN.Handlers;

public class ResolutionHandlers : IPtnHandler
{
    public void RegisterHandlers(IParseTorrentName parser)
    {
        parser.AddHandler(ResultKeys.Resolution, new Regex(@"\b[([]?4k[)\]]?\b", RegexOptions.IgnoreCase), Transformers.Value("4k"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Resolution, new Regex(@"21600?[pi]", RegexOptions.IgnoreCase), Transformers.Value("4k"), new() { SkipIfAlreadyFound = false, Remove = true });
        parser.AddHandler(ResultKeys.Resolution, new Regex(@"[([]?3840x\d{4}[)\]]?", RegexOptions.IgnoreCase), Transformers.Value("4k"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Resolution, new Regex(@"[([]?1920x\d{3,4}[)\]]?", RegexOptions.IgnoreCase), Transformers.Value("1080p"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Resolution, new Regex(@"[([]?1280x\d{3}[)\]]?", RegexOptions.IgnoreCase), Transformers.Value("720p"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Resolution, new Regex(@"[([]?\d{3,4}x(\d{3,4})[)\]]?", RegexOptions.IgnoreCase), Transformers.Value("$1p"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Resolution, new Regex(@"(480|720|1080)0[pi]", RegexOptions.IgnoreCase), Transformers.Value("$1p"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Resolution, new Regex(@"(?:BD|HD|M)(720|1080|2160)"), Transformers.Value("$1p"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Resolution, new Regex(@"(480|576|720|1080|2160)[pi]", RegexOptions.IgnoreCase), Transformers.Value("$1p"), new() { Remove = true });
        parser.AddHandler(ResultKeys.Resolution, new Regex(@"(?:^|\D)(\d{3,4})[pi]", RegexOptions.IgnoreCase), Transformers.Value("$1p"), new() { Remove = true });
    }
}