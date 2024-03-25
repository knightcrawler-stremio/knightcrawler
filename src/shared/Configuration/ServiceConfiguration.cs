namespace SharedContracts.Configuration;

public class ServiceConfiguration
{
    private const string Prefix = "COLLECTOR";
    private const string RealDebridApiKeyVariable = "REAL_DEBRID_API_KEY";
    public string RealDebridApiKey { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(RealDebridApiKeyVariable);
}
