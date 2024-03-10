namespace Producer.Features.Wordlists;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterWordCollections(this IServiceCollection services)
    {
        services.AddSingleton<IWordCollections, WordCollections>();
        services.AddHostedService<PopulationService>();

        return services;
    }
}
