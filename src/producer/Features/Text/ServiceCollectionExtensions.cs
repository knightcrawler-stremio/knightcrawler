namespace Producer.Features.Text;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAdultKeywordFilter(this IServiceCollection services, IConfiguration configuration)
    {
        var adultConfigSettings =
            services.LoadConfigurationFromConfig<AdultContentConfiguration>(configuration, AdultContentConfiguration.SectionName);

        if (adultConfigSettings.Allow)
        {
            return services;
        }
        
        return services.AddSingleton<IFuzzySearcher<string>>(
            _ =>
            {
                var options = new SearchOptions<string>
                {
                    Threshold = adultConfigSettings.Threshold,
                };

                return new FuzzyStringSearcher(adultConfigSettings.Keywords, options);
            });
    }
}