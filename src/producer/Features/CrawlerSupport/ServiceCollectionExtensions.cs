namespace Producer.Features.CrawlerSupport;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddCrawlers(this IServiceCollection services)
    {
        services.AddHttpClient(Literals.CrawlerClient);

        var crawlerTypes = Assembly.GetAssembly(typeof(ICrawler))
            .GetTypes()
            .Where(t => t is {IsClass: true, IsAbstract: false} && typeof(ICrawler).IsAssignableFrom(t));

        foreach (var type in crawlerTypes)
        {
            services.AddKeyedTransient(typeof(ICrawler), type.Name, type);
        }

        services
            .AddSingleton<ICrawlerProvider, CrawlerProvider>();

        return services;
    }
}
