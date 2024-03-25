namespace Producer.Features.Amqp;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection RegisterMassTransit(this IServiceCollection services)
    {
        var rabbitConfiguration = services.LoadConfigurationFromEnv<RabbitMqConfiguration>();
        
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.AutoStart = true;
                
                cfg.Host(rabbitConfiguration.Host, h =>
                {
                    h.Username(rabbitConfiguration.Username);
                    h.Password(rabbitConfiguration.Password);
                });

                cfg.Message<IngestTorrent>(e => e.SetEntityName(rabbitConfiguration.QueueName));
                cfg.Publish<IngestTorrent>(config => config.PublishToQueue(rabbitConfiguration.QueueName, rabbitConfiguration.QueueName));
                
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
