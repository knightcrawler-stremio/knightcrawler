namespace Producer.Models.Configuration;

public class PostgresConfiguration
{
    private const string Prefix = "POSTGRES";
    private const string HostVariable = "HOST";
    private const string UsernameVariable = "USER";
    private const string PasswordVariable = "PASSWORD";
    private const string DatabaseVariable = "DB";
    private const string PortVariable = "PORT";

    private string Host { get; init; } = Environment.GetEnvironmentVariable($"{Prefix}_{HostVariable}") ??
                                         throw new InvalidOperationException($"Environment variable {Prefix}_{HostVariable} is not set");

    private string Username { get; init; } = Environment.GetEnvironmentVariable($"{Prefix}_{UsernameVariable}") ??
                                             throw new InvalidOperationException($"Environment variable {Prefix}_{UsernameVariable} is not set");

    private string Password { get; init; } = Environment.GetEnvironmentVariable($"{Prefix}_{PasswordVariable}") ??
                                             throw new InvalidOperationException($"Environment variable {Prefix}_{PasswordVariable} is not set");

    private string Database { get; init; } = Environment.GetEnvironmentVariable($"{Prefix}_{DatabaseVariable}") ??
                                             throw new InvalidOperationException($"Environment variable {Prefix}_{DatabaseVariable} is not set");

    private int PORT { get; init; } = int.Parse(
        Environment.GetEnvironmentVariable($"{Prefix}_{PortVariable}") ??
        throw new InvalidOperationException($"Environment variable {Prefix}_{PortVariable} is not set"));

    public string StorageConnectionString => $"Host={Host};Port={PORT};Username={Username};Password={Password};Database={Database};";
}