using Literals = Producer.Features.JobSupport.Literals;

namespace Producer.Features.Crawlers.Nyaa;

[DisallowConcurrentExecution]
public class SyncNyaaJob(ICrawlerProvider crawlerProvider) : BaseJob(crawlerProvider)
{
    private const string JobName = nameof(NyaaCrawler);
    public static readonly JobKey Key = new(JobName, nameof(Literals.CrawlersJobs));
    public static readonly TriggerKey Trigger = new($"{JobName}-trigger", nameof(Literals.CrawlersJobs));
    protected override string Crawler => nameof(NyaaCrawler);
}
