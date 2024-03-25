namespace QBitCollector.Features.Qbit;

public class QbitConfiguration
{
    private const string Prefix = "QBIT";
    private const string ConnectionStringVariable = "HOST";
    
    public string? Host { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(ConnectionStringVariable);
}