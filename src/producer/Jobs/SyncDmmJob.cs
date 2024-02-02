using DebridMediaManagerCrawler = Producer.Crawlers.Sites.DebridMediaManagerCrawler;

namespace Producer.Jobs;

[DisallowConcurrentExecution]
public class SyncDmmJob(ICrawlerProvider crawlerProvider) : BaseJob(crawlerProvider)
{
    private const string JobName = nameof(DebridMediaManagerCrawler);
    public static readonly JobKey Key = new(JobName, nameof(Crawlers));
    public static readonly TriggerKey Trigger = new($"{JobName}-trigger", nameof(Crawlers));
    protected override string Crawler => nameof(DebridMediaManagerCrawler);
}