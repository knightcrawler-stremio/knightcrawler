namespace SharedContracts.Python;

public class PythonEngineService(ILogger<PythonEngineService> logger) : IHostedService
{
    private IntPtr _mainThreadState;
    private bool _isInitialized;
    
    public Task StartAsync(CancellationToken cancellationToken)
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
                logger.LogWarning("PYTHONNET_PYDLL env is not set. Exiting Application");
                Environment.Exit(1);
                return Task.CompletedTask;
            }

            Runtime.PythonDLL = pythonDllEnv;
            PythonEngine.Initialize();
            _mainThreadState = PythonEngine.BeginAllowThreads();
            
            _isInitialized = true;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to initialize Python engine");
            Environment.Exit(1);
        }
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        PythonEngine.EndAllowThreads(_mainThreadState);
        PythonEngine.Shutdown();
        
        return Task.CompletedTask;
    }
}