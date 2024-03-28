namespace SharedContracts.Python.RTN;

public class RankTorrentName : IRankTorrentName
{
    private readonly IPythonEngineService _pythonEngineService;
    private const string RtnModuleName = "RTN";

    private dynamic? _rtn;

    public RankTorrentName(IPythonEngineService pythonEngineService)
    {
        _pythonEngineService = pythonEngineService;
        InitModules();
    }
   
    public ParseTorrentTitleResponse Parse(string title) =>
        _pythonEngineService.ExecutePythonOperation(
            () =>
            {
                var result = _rtn?.parse(title);
                return ParseResult(result);
            }, nameof(Parse), throwOnErrors: false);
    
    private static ParseTorrentTitleResponse ParseResult(dynamic result)
    {
        if (result == null)
        {
            return new(false, string.Empty, 0);
        }
        
        var parsedTitle = result.GetAttr("parsed_title")?.As<string>() ?? string.Empty;
        var year = result.GetAttr("year")?.As<int>() ?? 0;
        var seasons = GetIntArray(result, "season");
        var episodes = GetIntArray(result, "episode");

        return new ParseTorrentTitleResponse(true, parsedTitle, year, seasons, episodes);
    }

    private static int[]? GetIntArray(dynamic result, string field)
    {
        var theList = result.GetAttr(field)?.As<PyList>();
        int[]? results = theList?.Length() > 0 ? theList.As<int[]>() : null;
        
        return results;
    }

    private void InitModules() => 
        _rtn = 
            _pythonEngineService.ExecutePythonOperation(() =>
        {
            _pythonEngineService.Sys.path.append(Path.Combine(AppContext.BaseDirectory, "python"));
            return Py.Import(RtnModuleName);
        }, nameof(InitModules), throwOnErrors: false);
}