namespace SharedContracts.Python.RTN;

public class RankTorrentName : IRankTorrentName
{
    private const string SysModuleName = "sys";
    private const string RtnModuleName = "RTN";

    private readonly ILogger<RankTorrentName> _logger;
    private dynamic? _sys;
    private dynamic? _rtn;

    public RankTorrentName(ILogger<RankTorrentName> logger)
    {
        _logger = logger;
        InitModules();
    }

   
    public ParseTorrentTitleResponse Parse(string title)
    {
        try
        {
            using var py = Py.GIL();
            var result = _rtn?.parse(title);

            if (result == null)
            {
                return new(false, string.Empty, 0);
            }

            return ParseResult(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to parse title");
            return new(false, string.Empty, 0);
        }
    }

    public bool IsTrash(string title)
    {
        try
        {
            using var py = Py.GIL();
            var result = _rtn?.check_trash(title);

            if (result == null)
            {
                return false;
            }

            var response = result.As<bool>() ?? false;
            
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to parse title");
            return false;
        }
    }
    
    public bool TitleMatch(string title, string checkTitle)
    {
        try
        {
            using var py = Py.GIL();
            var result = _rtn?.title_match(title, checkTitle);

            if (result == null)
            {
                return false;
            }

            var response = result.As<bool>() ?? false;
            
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to parse title");
            return false;
        }
    }
    
    
    private static ParseTorrentTitleResponse ParseResult(dynamic result)
    {
        var parsedTitle = result.GetAttr("parsed_title")?.As<string>() ?? string.Empty;
        var year = result.GetAttr("year")?.As<int>() ?? 0;
        var seasonList = result.GetAttr("season")?.As<PyList>();
        var episodeList = result.GetAttr("episode")?.As<PyList>();
        int[]? seasons = seasonList?.Length() > 0 ? seasonList.As<int[]>() : null;
        int[]? episodes = episodeList?.Length() > 0 ? episodeList.As<int[]>() : null;

        return new ParseTorrentTitleResponse(true, parsedTitle, year, seasons, episodes);
    }

    private void InitModules()
    {
        using var py = Py.GIL();
        _sys = Py.Import(SysModuleName);

        if (_sys == null)
        {
            _logger.LogError($"Failed to import Python module: {SysModuleName}");
            return;
        }

        _sys.path.append(Path.Combine(AppContext.BaseDirectory, "python"));

        _rtn = Py.Import(RtnModuleName);
        if (_rtn == null)
        {
            _logger.LogError($"Failed to import Python module: {RtnModuleName}");
        }
    }
}