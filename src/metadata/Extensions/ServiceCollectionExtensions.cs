namespace Metadata.Extensions;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient(HttpClients.ImdbDataClientName, client => client.BaseAddress = new(HttpClients.ImdbClientBaseAddress));

        return services;
    }

    internal static IServiceCollection AddRedis(this IServiceCollection services)
    {
        var redisConfiguration = services.LoadConfigurationFromEnv<RedisConfiguration>();

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString));
        services.AddTransient<ImdbRedisDbService>();

        return services;
    }

    internal static IServiceCollection AddJobSupport(this IServiceCollection services)
    {
        services.LoadConfigurationFromEnv<JobConfiguration>();

        services.AddScheduler()
            .AddTransient<DownloadImdbDataJob>()
            .AddHostedService<JobScheduler>();

        return services;
    }
}
