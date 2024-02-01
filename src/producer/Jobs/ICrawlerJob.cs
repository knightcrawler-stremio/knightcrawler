namespace Scraper.Jobs;

public interface ICrawlerJob<out TCrawler> : IJob
    where TCrawler : ICrawler
{
    TCrawler CrawlerType { get; }
}