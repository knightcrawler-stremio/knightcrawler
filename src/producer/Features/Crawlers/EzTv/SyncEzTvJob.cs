using Literals = Producer.Features.JobSupport.Literals;

namespace Producer.Features.Crawlers.EzTv;

[DisallowConcurrentExecution]
public class SyncEzTvJob(ICrawlerProvider crawlerProvider) : BaseJob(crawlerProvider)
{
    private const string JobName = nameof(EzTvCrawler);
    public static readonly JobKey Key = new(JobName, nameof(Literals.CrawlersJobs));
    public static readonly TriggerKey Trigger = new($"{JobName}-trigger", nameof(Literals.CrawlersJobs));
    protected override string Crawler => nameof(EzTvCrawler);
}
