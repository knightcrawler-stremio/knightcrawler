namespace Producer.Extensions;

internal static class ConfigureHostBuilderExtensions
{
    internal static IHostBuilder SetupSerilog(this ConfigureHostBuilder builder, IConfiguration configuration) =>
        builder.UseSerilog((_, c) =>
            c.ReadFrom.Configuration(configuration));
}