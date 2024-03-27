namespace SharedContracts.Python;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterPythonEngine(this IServiceCollection services)
    {
        services.AddSingleton<PythonEngineService>();
        
        services.AddHostedService(p => p.GetRequiredService<PythonEngineService>());

        return services;
    }
}