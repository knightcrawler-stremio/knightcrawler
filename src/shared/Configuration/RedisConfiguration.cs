namespace SharedContracts.Configuration;

public class RedisConfiguration
{
    private const string Prefix = "REDIS";
    private const string HostVariable = "HOST";
    private const string PortVariable = "PORT";
    private const string ExtraVariable = "EXTRA";

    private string Host { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(HostVariable);
    private int PORT { get; init; } = Prefix.GetEnvironmentVariableAsInt(PortVariable, 6379);
    private string EXTRA { get; init; } = Prefix.GetOptionalEnvironmentVariableAsString(ExtraVariable, "?abortConnect=false,allowAdmin=true");
    public string ConnectionString => $"{Host}:{PORT}{EXTRA}";
}