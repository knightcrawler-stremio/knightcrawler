namespace Metadata.Extensions;

internal static class WebApplicationBuilderExtensions
{
    internal static IHostBuilder SetupSerilog(this WebApplicationBuilder builder, IConfiguration configuration) =>
        builder.Host.UseSerilog((_, c) =>
            c.ReadFrom.Configuration(configuration));

    internal static WebApplicationBuilder SetupWolverine(this WebApplicationBuilder builder)
    {
        builder.Host.UseWolverine(
            options =>
            {
                options.DefaultExecutionTimeout = 6.Hours();
            });

        return builder;
    }
}
