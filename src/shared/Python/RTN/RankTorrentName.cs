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
        _pythonEngineService.ExecutePythonOperationWithDefault(
            () =>
            {
                var result = _rtn?.parse(title);
                return ParseResult(result);
            }, new ParseTorrentTitleResponse(false, null), nameof(Parse), throwOnErrors: false, logErrors: false);
    
    private static ParseTorrentTitleResponse ParseResult(dynamic result)
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
        
        var response = JsonSerializer.Deserialize<RtnResponse>(json);

        return new(true, response);
    }

    private void InitModules() => 
        _rtn = 
            _pythonEngineService.ExecutePythonOperation(() =>
        {
            _pythonEngineService.Sys.path.append(Path.Combine(AppContext.BaseDirectory, "python"));
            return Py.Import(RtnModuleName);
        }, nameof(InitModules), throwOnErrors: false);
}