namespace Producer.Interfaces;

public interface ICrawlerProvider
{
    IEnumerable<ICrawler> GetAll();
    
    ICrawler Get(string name);
}