namespace Metadata.Features.Configuration;

public class MongoConfiguration
{
    private const string Prefix = "MONGODB";
    private const string HostVariable = "HOST";
    private const string PortVariable = "PORT";
    private const string DbVariable = "DB";
    private const string UsernameVariable = "USER";
    private const string PasswordVariable = "PASSWORD";
    

    private string Host { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(HostVariable);
    private int Port { get; init; } = Prefix.GetEnvironmentVariableAsInt(PortVariable, 27017);
    private string Username { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(UsernameVariable);
    private string Password { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(PasswordVariable);
    public string DbName { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(DbVariable);
    
    public string ConnectionString => $"mongodb://{Username}:{Password}@{Host}:{Port}/{DbName}?tls=false&directConnection=true&authSource=admin";
}