namespace Metadata.Features.Configuration;

public class JobConfiguration
{
    private const string Prefix = "METADATA";
    private const string DownloadImdbDataVariable = "DOWNLOAD_IMDB_DATA_SCHEDULE";
    private const string DownloadImdbDataOnceVariable = "DOWNLOAD_IMDB_DATA_ONCE";

    public string DownloadImdbCronSchedule { get; init; } = Prefix.GetOptionalEnvironmentVariableAsString(DownloadImdbDataVariable, CronExpressions.EveryHour);
    public bool DownloadImdbOnce { get; init; } = Prefix.GetEnvironmentVariableAsBool(DownloadImdbDataOnceVariable);
}