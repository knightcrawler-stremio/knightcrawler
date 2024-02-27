using Producer.Crawlers.Sites;

namespace Producer.Jobs;

[DisallowConcurrentExecution]
public class SyncNyaaJob(ICrawlerProvider crawlerProvider) : BaseJob(crawlerProvider)
{
    private const string JobName = nameof(NyaaCrawler);
    public static readonly JobKey Key = new(JobName, nameof(Crawlers));
    public static readonly TriggerKey Trigger = new($"{JobName}-trigger", nameof(Crawlers));
    protected override string Crawler => nameof(NyaaCrawler);
}