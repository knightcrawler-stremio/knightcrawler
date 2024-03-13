using Literals = Producer.Features.JobSupport.Literals;

namespace Producer.Features.Crawlers.Torrentio;

[DisallowConcurrentExecution]
[ManualJobRegistration]
public class SyncTorrentioJob(ICrawlerProvider crawlerProvider) : BaseJob(crawlerProvider)
{
    private const string JobName = nameof(TorrentioCrawler);
    public static readonly JobKey Key = new(JobName, nameof(Literals.CrawlersJobs));
    public static readonly TriggerKey Trigger = new($"{JobName}-trigger", nameof(Literals.CrawlersJobs));
    protected override string Crawler => nameof(TorrentioCrawler);
}
