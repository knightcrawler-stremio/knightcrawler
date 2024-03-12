namespace Tissue.Features.DataProcessing;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddDataStorage(this IServiceCollection services)
    {
        services.LoadConfigurationFromEnv<PostgresConfiguration>();
        services.AddTransient<IDataStorage, DapperDataStorage>();

        return services;
    }
}
