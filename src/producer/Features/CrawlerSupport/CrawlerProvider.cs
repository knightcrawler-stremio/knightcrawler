namespace Producer.Features.CrawlerSupport;

public class CrawlerProvider(IServiceProvider serviceProvider) : ICrawlerProvider
{
    public IEnumerable<ICrawler> GetAll() =>
        serviceProvider.GetServices<ICrawler>();
    
    public ICrawler Get(string name) => 
        serviceProvider.GetRequiredKeyedService<ICrawler>(name);

}