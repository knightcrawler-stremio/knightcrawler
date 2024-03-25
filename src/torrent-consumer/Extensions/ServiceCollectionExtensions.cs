namespace TorrentConsumer.Extensions;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.LoadConfigurationFromEnv<PostgresConfiguration>();
        services.AddTransient<IDataStorage, DapperDataStorage>();
        return services;
    }
    internal static IServiceCollection RegisterMassTransit(this IServiceCollection services)
    {
        var rabbitConfiguration = services.LoadConfigurationFromEnv<RabbitMqConfiguration>();
        var redisConfiguration = services.LoadConfigurationFromEnv<RedisConfiguration>();
        
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.AutoStart = true;

                cfg.Host(
                    rabbitConfiguration.Host, h =>
                    {
                        h.Username(rabbitConfiguration.Username);
                        h.Password(rabbitConfiguration.Password);
                    });
                
                cfg.Message<IngestTorrent>(e => e.SetEntityName(rabbitConfiguration.QueueName));
                
                cfg.SetupCollectMetadata(rabbitConfiguration);

                cfg.ConfigureEndpoints(context);
            });

            x.RegisterTorrentIngestionSaga(redisConfiguration, rabbitConfiguration);
        });
        
        return services;
    }

    private static void SetupCollectMetadata(this IRabbitMqBusFactoryConfigurator cfg, RabbitMqConfiguration rabbitConfiguration)
    {
        cfg.Message<CollectMetadata>(e => e.SetEntityName(rabbitConfiguration.CollectorsExchange));
        cfg.Publish<CollectMetadata>(
            config =>
            {
                config.Durable = true;
                config.AutoDelete = false;
                config.ExchangeType = "fanout";

                if (rabbitConfiguration.DebridEnabled)
                {
                    config.BindQueue(rabbitConfiguration.CollectorsExchange, rabbitConfiguration.DebridCollectorQueueName, opt => opt.Durable = true);
                }

                if (rabbitConfiguration.QbitEnabled)
                {
                    config.BindQueue(rabbitConfiguration.CollectorsExchange, rabbitConfiguration.QbitCollectorQueueName, opt => opt.Durable = true);
                }
            });
    }
    
    private static void RegisterTorrentIngestionSaga(this IBusRegistrationConfigurator x, RedisConfiguration redisConfiguration, RabbitMqConfiguration rabbitMqConfiguration)
    {
        x.AddSagaStateMachine<TorrentIngestionSagaStateMachine, TorrentIngestionState>(
                cfg =>
                {
                    cfg.UseMessageRetry(r => r.Intervals(1000,2000,5000));
                    cfg.UseInMemoryOutbox();
                })
            .RedisRepository(redisConfiguration.ConnectionString)
            .Endpoint(
                e =>
                {
                    e.Name = rabbitMqConfiguration.QueueName;
                    e.ConcurrentMessageLimit = 50;
                    e.PrefetchCount = 50;
                });

        x.AddConsumer<PerformIngestionConsumer>();
    }

    internal static IServiceCollection AddServiceConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IParseTorrentTitle, ParseTorrentTitle>();
        
        return services;
    }
}
