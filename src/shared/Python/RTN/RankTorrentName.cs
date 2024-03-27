namespace SharedContracts.Python.RTN;

public class RankTorrentName : IRankTorrentName
{
    private readonly ILogger<RankTorrentName> _logger;
    private dynamic? _sys;
    private dynamic? _rtn;

    public RankTorrentName(ILogger<RankTorrentName> logger)
    {
        _logger = logger;
        SetupVariables();
    }

    public ParseTorrentTitleResponse Parse(string title)
    {
        try
        {
            using var py = Py.GIL();
            var result = _rtn.parse(title);

            if (result == null)
            {
                return new(false, string.Empty);
            }

            var parsedTitle = result.GetAttr("parsed_title").As<string>();
            var yearList = result.GetAttr("year").As<PyList>();
            var seasonList = result.GetAttr("season").As<PyList>();
            var episodeList = result.GetAttr("episode").As<PyList>();
            int? year = yearList.Length() > 0 ? yearList[0].As<int>() : null;
            int[]? seasons = seasonList.Length() > 0 ? seasonList.As<int[]>() : null;
            int[]? episodes = episodeList.Length() > 0 ? episodeList.As<int[]>() : null;

            return new ParseTorrentTitleResponse(true, parsedTitle, year, seasons, episodes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to parse title");
            return new(false, string.Empty);
        }
    }

    private void SetupVariables()
    {
        using var py = Py.GIL();
        _sys = Py.Import("sys");
        _sys.path.append(Path.Combine(AppContext.BaseDirectory, "python"));
        _rtn = Py.Import("RTN");
    }
}