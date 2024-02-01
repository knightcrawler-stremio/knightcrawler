namespace Scraper.Jobs;

[DisallowConcurrentExecution]
public class SyncTgxJob(ICrawlerProvider crawlerProvider) : BaseJob(crawlerProvider)
{
    private const string JobName = nameof(TgxCrawler);
    public static readonly JobKey Key = new(JobName, nameof(Crawlers));
    public static readonly TriggerKey Trigger = new($"{JobName}-trigger", nameof(Crawlers));
    protected override string Crawler => nameof(TgxCrawler);
}