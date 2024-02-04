namespace Producer.Models.Configuration;

public class RabbitMqConfiguration
{
    public const string SectionName = "RabbitMqConfiguration";
    public const string Filename = "rabbitmq.json";
    
    public string? Host { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? QueueName { get; set; }
    public bool Durable { get; set; }
    public int MaxQueueSize { get; set; }
    public int MaxPublishBatchSize { get; set; } = 500;
    public int PublishIntervalInSeconds { get; set; } = 1000 * 10;

    public void Validate()
    {
        if (MaxQueueSize == 0)
        {
            return;
        }
        
        if (MaxQueueSize < 0)
        {
            throw new InvalidOperationException("MaxQueueSize cannot be less than 0 in RabbitMqConfiguration");
        }
        
        if (MaxPublishBatchSize < 0)
        {
            throw new InvalidOperationException("MaxPublishBatchSize cannot be less than 0 in RabbitMqConfiguration");
        }

        if (MaxPublishBatchSize > MaxQueueSize)
        {
            throw new InvalidOperationException("MaxPublishBatchSize cannot be greater than MaxQueueSize in RabbitMqConfiguration");
        }
    }
}