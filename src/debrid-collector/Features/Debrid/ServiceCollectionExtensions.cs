using DebridCollector.Features.Configuration;

namespace DebridCollector.Features.Debrid;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRealDebridClient(this IServiceCollection services, DebridCollectorConfiguration serviceConfiguration)
    {
        services.AddHttpClient<IDebridHttpClient, RealDebridClient>(
                client =>
                {
                    client.BaseAddress = new("https://api.real-debrid.com/rest/1.0/");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {serviceConfiguration.RealDebridApiKey}");
                })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }
    
    private static AsyncPolicy<HttpResponseMessage> GetRetryPolicy(int MaxRetryCount = 5, int MaxJitterTime = 1000) =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(MaxRetryCount, RetryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, RetryAttempt)) +
                TimeSpan.FromMilliseconds(Random.Shared.Next(0, MaxJitterTime)));

    private static AsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 5, TimeSpan.FromSeconds(30));
}