namespace SharedContracts.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void DisableIpPortBinding(this WebApplicationBuilder builder) =>
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(0, listenOptions =>
            {
                listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.None;
            });
        });
}