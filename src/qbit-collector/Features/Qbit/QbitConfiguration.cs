namespace QBitCollector.Features.Qbit;

public class QbitConfiguration
{
    private const string Prefix = "QBIT";
    private const string HOST_VARIABLE = "HOST";
    private const string TRACKERS_URL_VARIABLE = "TRACKERS_URL";
    private const string CONCURRENCY_VARIABLE = "CONCURRENCY";
    
    public string? Host { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(HOST_VARIABLE);
    public string? TrackersUrl { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(TRACKERS_URL_VARIABLE);
    
    public int Concurrency { get; init; } = Prefix.GetEnvironmentVariableAsInt(CONCURRENCY_VARIABLE, 8);
}