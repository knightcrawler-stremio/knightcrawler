namespace Scraper.Models;

public class RabbitMqConfiguration
{
    public const string SectionName = "RabbitMqConfiguration";
    public const string Filename = "rabbitmq.json";
    
    public string? Host { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? QueueName { get; set; }
    public bool Durable { get; set; }
}