namespace QBitCollector.Features.Qbit;

public class QbitConfiguration
{
    private const string Prefix = "QBIT";
    private const string HOST_VARIABLE = "HOST";
    private const string TRACKERS_URL_VARIABLE = "TRACKERS_URL";
    
    public string? Host { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(HOST_VARIABLE);
    public string? TrackersUrl { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(TRACKERS_URL_VARIABLE);
}