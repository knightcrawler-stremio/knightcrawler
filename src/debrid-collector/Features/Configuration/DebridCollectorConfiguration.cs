namespace DebridCollector.Features.Configuration;

public class DebridCollectorConfiguration
{
    private const string Prefix = "COLLECTOR";
    private const string RealDebridApiKeyVariable = "REAL_DEBRID_API_KEY";
    public string RealDebridApiKey { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(RealDebridApiKeyVariable);
}
