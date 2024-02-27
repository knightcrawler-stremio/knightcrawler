namespace Metadata.Features.DownloadImdbData;

public class DownloadImdbDataJob(IMessageBus messageBus, JobConfiguration configuration) : BaseJob
{
    public override bool IsScheduelable => !configuration.DownloadImdbOnce && !string.IsNullOrEmpty(configuration.DownloadImdbCronSchedule);
    public override string JobName => nameof(DownloadImdbDataJob);
    public override async Task Invoke() => await messageBus.SendAsync(new GetImdbDataRequest());
}