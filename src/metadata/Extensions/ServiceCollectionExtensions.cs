namespace Metadata.Extensions;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient(HttpClients.ImdbDataClientName, client => client.BaseAddress = new(HttpClients.ImdbClientBaseAddress));

        return services;
    }
    
    internal static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        services.LoadConfigurationFromEnv<MongoConfiguration>();
        services.AddTransient<ImdbMongoDbService>();
        
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