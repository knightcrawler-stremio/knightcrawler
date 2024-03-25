var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddServiceConfiguration();
builder.SetupSerilog(builder.Configuration);
builder.SetupWolverine();

builder.Services
    .AddHttpClients()
    .AddServiceConfiguration()
    .AddDatabase()
    .AddImporters();

var host = builder.Build();

return await host.RunOaktonCommands(args);
