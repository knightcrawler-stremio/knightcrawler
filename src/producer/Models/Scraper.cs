namespace Scraper.Models;

public class Scraper
{
    public string? Name { get; set; }

    public int IntervalSeconds { get; set; } = 60;

    public bool Enabled { get; set; } = true;
}