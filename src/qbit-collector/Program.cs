var builder = WebApplication.CreateBuilder();

builder.DisableIpPortBinding();

builder.Configuration
    .AddServiceConfiguration();

builder.Host
    .SetupSerilog(builder.Configuration);

builder.Services
    .AddServiceConfiguration()
    .AddDatabase()
    .AddRedisCache()
    .RegisterMassTransit();

var app = builder.Build();
app.Run();
