namespace Producer.Features.Amqp;

public class RabbitMqConfiguration
{
    private const string Prefix = "RABBITMQ";
    private const string HostVariable = "HOST";
    private const string UsernameVariable = "USER";
    private const string PasswordVariable = "PASSWORD";
    private const string QueueNameVariable = "QUEUE_NAME";
    private const string DurableVariable = "DURABLE";
    private const string MaxQueueSizeVariable = "MAX_QUEUE_SIZE";
    private const string MaxPublishBatchSizeVariable = "MAX_PUBLISH_BATCH_SIZE";
    private const string PublishIntervalInSecondsVariable = "PUBLISH_INTERVAL_IN_SECONDS";
    
    public string? Username { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(UsernameVariable);
    public string? Host { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(HostVariable);
    public string? Password { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(PasswordVariable);
    public string? QueueName { get; init; } = Prefix.GetRequiredEnvironmentVariableAsString(QueueNameVariable);
    public bool Durable { get; init; } = Prefix.GetEnvironmentVariableAsBool(DurableVariable, true);
    public int MaxQueueSize { get; init; } = Prefix.GetEnvironmentVariableAsInt(MaxQueueSizeVariable, 0);
    public int MaxPublishBatchSize { get; set; } = Prefix.GetEnvironmentVariableAsInt(MaxPublishBatchSizeVariable, 500);
    public int PublishIntervalInSeconds { get; set; } = Prefix.GetEnvironmentVariableAsInt(PublishIntervalInSecondsVariable, 1000 * 10);

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