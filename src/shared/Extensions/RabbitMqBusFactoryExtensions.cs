namespace SharedContracts.Extensions;

public static class RabbitMqBusFactoryExtensions
{
    public static IRabbitMqBusFactoryConfigurator SetupExchangeEndpoint(this IRabbitMqBusFactoryConfigurator cfg,
        string exchangeName,
        bool durable = true,
        bool autoDelete = false,
        string type = "fanout")
    {
        cfg.ReceiveEndpoint(
            exchangeName, e =>
            {
                e.Bind(
                    exchangeName, config =>
                    {
                        config.Durable = durable;
                        config.AutoDelete = autoDelete;
                        config.ExchangeType = type;
                    });
            });

        return cfg;
    }

    public static IRabbitMqMessagePublishTopologyConfigurator<TMessage> PublishToQueue<TMessage>(this IRabbitMqMessagePublishTopologyConfigurator<TMessage> cfg,
        string exchangeName,
        string queueName,
        bool durable = true,
        bool autoDelete = false,
        string type = "fanout") where TMessage : class
    {
        cfg.Durable = durable;
        cfg.AutoDelete = autoDelete;
        cfg.ExchangeType = type;
        cfg.BindQueue(
            exchangeName, queueName, options =>
            {
                options.Durable = durable;
            });
        
        return cfg;
    }
}