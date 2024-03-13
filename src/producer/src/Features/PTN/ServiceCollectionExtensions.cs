namespace Producer.Features.PTN;

public static class ServiceCollectionExtensions
{
    public static void AddParseTorrentName(this IServiceCollection services, params Assembly[]? additionalAssemblies)
    {
        var assemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };

        if (additionalAssemblies != null)
        {
            assemblies.AddRange(additionalAssemblies);
        }

        foreach (var assembly in assemblies)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces()
                    .Contains(typeof(IPtnHandler)) && !t.IsAbstract);

            foreach (var type in handlerTypes)
            {
                services.AddSingleton(typeof(IPtnHandler), type);
            }
        }

        services.AddSingleton<IParseTorrentName, ParseTorrentName>();
    }
}