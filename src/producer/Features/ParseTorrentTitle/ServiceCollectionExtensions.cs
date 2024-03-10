namespace Producer.Features.ParseTorrentTitle;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterParseTorrentTitle(this IServiceCollection services)
    {
        services.AddSingleton<IParsingService, ParsingService>();
        services.AddSingleton<ITorrentTitleParser, TorrentTitleParser>();

        return services;
    }
}
