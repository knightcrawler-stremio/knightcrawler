namespace Producer.Features.DataProcessing;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddDataStorage(this IServiceCollection services)
    {
        services.LoadConfigurationFromEnv<PostgresConfiguration>();
        services.LoadConfigurationFromEnv<MongoConfiguration>();
        services.AddTransient<IDataStorage, DapperDataStorage>();
        services.AddTransient<IMessagePublisher, TorrentPublisher>();
        services.AddSingleton<ImdbMongoDbService>();
        return services;
    }
}