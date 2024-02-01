namespace Scraper.Jobs;

[DisallowConcurrentExecution]
public class SyncTpbJob(ICrawlerProvider crawlerProvider) : BaseJob(crawlerProvider)
{
    private const string JobName = nameof(TpbCrawler);
    public static readonly JobKey Key = new(JobName, nameof(Crawlers));
    public static readonly TriggerKey Trigger = new($"{JobName}-trigger", nameof(Crawlers));
    protected override string Crawler => nameof(TpbCrawler);
}