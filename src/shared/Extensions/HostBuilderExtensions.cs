namespace SharedContracts.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder SetupSerilog(this IHostBuilder builder, IConfiguration configuration) =>
        builder.UseSerilog((_, c) =>
            c.ReadFrom.Configuration(configuration));
}
