namespace SharedContracts.Python;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterPythonEngine(this IServiceCollection services)
    {
        services.AddSingleton<IPythonEngineService, PythonEngineService>();
        services.AddHostedService<PythonEngineManager>();

        return services;
    }
}