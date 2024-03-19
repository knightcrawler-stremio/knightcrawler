var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddServiceConfiguration();
builder.SetupSerilog(builder.Configuration);
builder.SetupWolverine();

builder.Services
    .AddHttpClients()
    .AddServiceConfiguration()
    .AddDatabase();

var host = builder.Build();

await host.RunAsync();
