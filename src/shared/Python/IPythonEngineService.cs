namespace SharedContracts.Python;

public interface IPythonEngineService
{
    ILogger<PythonEngineService> Logger { get; }

    Task InitializePythonEngine(CancellationToken cancellationToken);
    T ExecuteCommandOrScript<T>(string command, PyModule module, bool throwOnErrors);
    T ExecutePythonOperation<T>(Func<T> operation, string operationName, bool throwOnErrors);
    T ExecutePythonOperationWithDefault<T>(Func<T> operation, T? defaultValue, string operationName, bool throwOnErrors, bool logErrors);
    Task StopPythonEngine(CancellationToken cancellationToken);
    dynamic? Sys { get; }
}