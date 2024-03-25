namespace Producer.Features.DataProcessing;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddDataStorage(this IServiceCollection services)
    {
        services.LoadConfigurationFromEnv<PostgresConfiguration>();
        var redisConfiguration = services.LoadConfigurationFromEnv<RedisConfiguration>();
       
        services.AddTransient<IDataStorage, DapperDataStorage>();
        services.AddTransient<IMessagePublisher, TorrentPublisher>();
        services.AddSingleton<IParseTorrentTitle, ParseTorrentTitle>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfiguration.ConnectionString;
            options.InstanceName = "producer-imdb-cache:";
        });
        
        return services;
    }
}
