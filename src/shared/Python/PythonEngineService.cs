namespace SharedContracts.Python;

public class PythonEngineService(ILogger<PythonEngineService> logger) : IPythonEngineService
{
    private IntPtr _mainThreadState;
    private bool _isInitialized;

    public ILogger<PythonEngineService> Logger { get; } = logger;

    public dynamic? Sys { get; private set; }

    public Task InitializePythonEngine(CancellationToken cancellationToken)
    {
        if (_isInitialized)
        {
            return Task.CompletedTask;
        }

        try
        {
            var pythonDllEnv = Environment.GetEnvironmentVariable("PYTHONNET_PYDLL");

            if (string.IsNullOrWhiteSpace(pythonDllEnv))
            {
                Logger.LogWarning("PYTHONNET_PYDLL env is not set. Exiting Application");
                Environment.Exit(1);
                return Task.CompletedTask;
            }

            Runtime.PythonDLL = pythonDllEnv;
            PythonEngine.Initialize();
            _mainThreadState = PythonEngine.BeginAllowThreads();

            _isInitialized = true;
            Logger.LogInformation("Python engine initialized");
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"Failed to initialize Python engine: {e.Message}");
            Environment.Exit(1);
        }

        return Task.CompletedTask;
    }

    public T ExecuteCommandOrScript<T>(string command, PyModule module, bool throwOnErrors) =>
        ExecutePythonOperation(
            () =>
            {
                var pyCompile = PythonEngine.Compile(command);
                var nativeResult = module.Execute(pyCompile);
                return nativeResult.As<T>();
            }, nameof(ExecuteCommandOrScript), throwOnErrors);

    public T ExecutePythonOperation<T>(Func<T> operation, string operationName, bool throwOnErrors) =>
        ExecutePythonOperationWithDefault(operation, default, operationName, throwOnErrors);

    public T ExecutePythonOperationWithDefault<T>(Func<T> operation, T? defaultValue, string operationName, bool throwOnErrors) =>
        ExecutePythonOperationInternal(operation, defaultValue, operationName, throwOnErrors);

    public void ExecuteOnGIL(Action act, bool throwOnErrors)
    {
        Sys ??= LoadSys();

        try
        {
            using var gil = Py.GIL();
            act();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Python Error: {Message} ({OperationName})", ex.Message, nameof(ExecuteOnGIL));

            if (throwOnErrors)
            {
                throw;
            }
        }
    }

    public Task StopPythonEngine(CancellationToken cancellationToken)
    {
        PythonEngine.EndAllowThreads(_mainThreadState);
        PythonEngine.Shutdown();

        return Task.CompletedTask;
    }

    private static dynamic LoadSys()
    {
        using var gil = Py.GIL();
        var sys = Py.Import("sys");
        
        return sys;
    }

    // ReSharper disable once EntityNameCapturedOnly.Local
    private T ExecutePythonOperationInternal<T>(Func<T> operation, T? defaultValue, string operationName, bool throwOnErrors)
    {
        Sys ??= LoadSys();

        var result = defaultValue;

        try
        {
            using var gil = Py.GIL();
            result = operation();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Python Error: {Message} ({OperationName})", ex.Message, nameof(operationName));

            if (throwOnErrors)
            {
                throw;
            }
        }

        return result;
    }
}