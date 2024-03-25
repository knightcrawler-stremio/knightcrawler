namespace SharedContracts.Configuration;

public class RedisConfiguration
{
    private const string Prefix = "REDIS";
    private const string ConnectionStringVariable = "CONNECTION_STRING";
    
    public string? ConnectionString { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(ConnectionStringVariable) + ",abortConnect=false,allowAdmin=true";
}