namespace Metadata.Extensions;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient(HttpClients.ImdbDataClientName, client => client.BaseAddress = new(HttpClients.ImdbClientBaseAddress));

        return services;
    }

    internal static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.LoadConfigurationFromEnv<PostgresConfiguration>();
        services.AddScoped<ImdbDbService>();
        
        return services;
    }
    
    internal static IServiceCollection AddImporters(this IServiceCollection services)
    {
        services.AddScoped<IFileImport<ImdbBasicEntry>, BasicsFile>();
        services.AddScoped<IFileImport<ImdbAkaEntry>, AkasFile>();
        services.AddScoped<IFileImport<ImdbEpisodeEntry>, EpisodesFile>();
        
        return services;
    }
    
    internal static IServiceCollection AddServiceConfiguration(this IServiceCollection services)
    {
        services.LoadConfigurationFromEnv<ServiceConfiguration>();
        services.AddScoped<IImdbFileDownloader, ImdbFileDownloader>();
        services.AddHostedService<DownloadImdbDataJob>();
        
        return services;
    }
}
