namespace Tissue.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(
            loggingBuilder =>
            {
                loggingBuilder.ClearProviders();

                var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

                loggingBuilder.AddSerilog(logger);
            });

        return services;
    }

    public static IServiceCollection AddKleenexService(this IServiceCollection services)
    {
        services.AddHostedService<KleenexService>();

        return services;
    }
}
