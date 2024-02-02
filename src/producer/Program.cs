using Producer.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddScrapeConfiguration();

builder.Host
    .SetupSerilog(builder.Configuration);

builder.Services
    .RegisterMassTransit(builder.Configuration)
    .AddDataStorage()
    .AddCrawlers()
    .AddQuartz(builder.Configuration);

var host = builder.Build();

await host.RunAsync();