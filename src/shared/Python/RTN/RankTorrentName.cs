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

    public ParseTorrentTitleResponse Parse(string title, bool trashGarbage = true, bool logErrors = false, bool throwOnErrors = false)
    {
        try
        {
            using var gil = Py.GIL();
            var result = _rtn?.parse(title, trashGarbage);
            return ParseResult(result);
        }
        catch (Exception ex)
        {
            if (logErrors)
            {
                _pythonEngineService.Logger.LogError(ex, "Python Error: {Message} ({OperationName})", ex.Message, nameof(Parse));
            }
            
            if (throwOnErrors)
            {
                throw;
            }

            return new(false, null);
        }
    }
    
    public List<ParseTorrentTitleResponse?> BatchParse(IReadOnlyCollection<string> titles, int chunkSize = 500, int workers = 20, bool trashGarbage = true, bool logErrors = false, bool throwOnErrors = false)
    {
        var responses = new List<ParseTorrentTitleResponse?>();

        try
        {
            if (titles.Count == 0)
            {
                return responses;
            }

            using var gil = Py.GIL();
            var pythonList = new PyList(titles.Select(x => new PyString(x).As<PyObject>()).ToArray());
            PyList results = _rtn?.batch_parse(pythonList, trashGarbage, chunkSize, workers);

            if (results == null)
            {
                return responses;
            }

            responses.AddRange(results.Select(ParseResult));
        }
        catch (Exception ex)
        {
            if (logErrors)
            {
                _pythonEngineService.Logger.LogError(ex, "Python Error: {Message} ({OperationName})", ex.Message, nameof(Parse));
            }
            
            if (throwOnErrors)
            {
                throw;
            }
        }

        return responses;
    }

    private static ParseTorrentTitleResponse? ParseResult(dynamic result)
    {
        try
        {
            if (result == null)
            {
                return new(false, null);
            }

            var json = result.model_dump_json()?.As<string?>();

            if (json is null || string.IsNullOrEmpty(json))
            {
                return new(false, null);
            }

            var mediaType = result.GetAttr("type")?.As<string>();
        
            if (string.IsNullOrEmpty(mediaType))
            {
                return new(false, null);
            }
        
            var response = JsonSerializer.Deserialize<RtnResponse>(json);
        
            response.IsMovie = mediaType.Equals("movie", StringComparison.OrdinalIgnoreCase);
        
            return new(true, response);
        }
        catch
        {
            return new(false, null);
        }
    }

    private void InitModules() => 
        _rtn = 
            _pythonEngineService.ExecutePythonOperation(() =>
        {
            _pythonEngineService.Sys.path.append(Path.Combine(AppContext.BaseDirectory, "python"));
            return Py.Import(RtnModuleName);
        }, nameof(InitModules), throwOnErrors: false);
}