namespace SharedContracts.Python;

public class PythonEngineManager(IPythonEngineService pythonEngineService) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => pythonEngineService.InitializePythonEngine(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) => pythonEngineService.StopPythonEngine(cancellationToken);
}