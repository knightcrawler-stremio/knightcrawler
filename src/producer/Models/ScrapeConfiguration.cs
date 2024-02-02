namespace Producer.Models;

public class ScrapeConfiguration
{
    public const string SectionName = "ScrapeConfiguration";
    public const string Filename = "scrapers.json";
    
    public List<Scraper> Scrapers { get; set; } = [];
    public string StorageConnectionString { get; set; } = "";
}