var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddServiceConfiguration();
builder.SetupSerilog(builder.Configuration);
builder.SetupWolverine();

builder.Services
    .AddHttpClients()
    .AddJobSupport();

var host = builder.Build();

await host.RunAsync();
