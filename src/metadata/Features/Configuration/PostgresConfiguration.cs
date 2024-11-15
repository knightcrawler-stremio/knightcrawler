namespace Metadata.Features.Configuration;

public class PostgresConfiguration
{
    private const string Prefix = "POSTGRES";
    private const string HostVariable = "HOST";
    private const string UsernameVariable = "USER";
    private const string PasswordVariable = "PASSWORD";
    private const string DatabaseVariable = "DB";
    private const string PortVariable = "PORT";
    private const string CommandTimeoutVariable = "COMMAND_TIMEOUT_SEC"; // Seconds

    private string Host { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(HostVariable);
    private string Username { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(UsernameVariable);
    private string Password { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(PasswordVariable);
    private string Database { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(DatabaseVariable);
    private int PORT { get; init; } = Prefix.GetEnvironmentVariableAsInt(PortVariable, 5432);
    private int CommandTimeout { get; init; } = Prefix.GetEnvironmentVariableAsInt(CommandTimeoutVariable, 300);

    public string StorageConnectionString => $"Host={Host};Port={PORT};Username={Username};Password={Password};Database={Database};CommandTimeout={CommandTimeout}";
}
