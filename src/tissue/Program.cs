var builder = Host.CreateApplicationBuilder();

builder.Configuration
    .AddScrapeConfiguration();

builder.Services
    .AddDataStorage()
    .RegisterWordCollections()
    .AddSerilogLogging(builder.Configuration)
    .AddKleenexService();

var host = builder.Build();

await host.RunAsync();
