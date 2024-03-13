namespace Producer.Features.CrawlerSupport;

public interface ICrawlerProvider
{
    IEnumerable<ICrawler> GetAll();

    ICrawler Get(string name);
}
