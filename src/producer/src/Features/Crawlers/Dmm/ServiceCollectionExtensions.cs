namespace Producer.Features.Crawlers.Dmm;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDmmSupport(this IServiceCollection services)
    {
        services.AddHttpClient<IDMMFileDownloader, DMMFileDownloader>(DMMFileDownloader.ClientName, client =>
        {
            client.BaseAddress = new("https://github.com/debridmediamanager/hashlists/zipball/main/");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("curl");
        });

        return services;
    }
}