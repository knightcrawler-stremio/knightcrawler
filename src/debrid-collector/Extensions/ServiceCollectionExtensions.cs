using DebridCollector.Features.Configuration;

namespace DebridCollector.Extensions;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.LoadConfigurationFromEnv<PostgresConfiguration>();
        services.AddTransient<IDataStorage, DapperDataStorage>();
        
        return services;
    }
    
    internal static IServiceCollection AddServiceConfiguration(this IServiceCollection services)
    {
        var serviceConfiguration = services.LoadConfigurationFromEnv<DebridCollectorConfiguration>();
        
        services.AddRealDebridClient(serviceConfiguration);
        services.AddSingleton<IParseTorrentTitle, ParseTorrentTitle>();
        services.AddHostedService<DebridRequestProcessor>();

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
                
                cfg.Message<CollectMetadata>(e => e.SetEntityName(rabbitConfiguration.DebridCollectorQueueName));
                cfg.ConfigureEndpoints(context);
            });

            x.AddConsumer<PerformMetadataRequestConsumer>();
            x.AddConsumer<WriteMetadataConsumer>();
            x.RegisterMetadataIngestionSaga(redisConfiguration, rabbitConfiguration);
        });
        
        return services;
    }
    
    private static void RegisterMetadataIngestionSaga(this IBusRegistrationConfigurator x, RedisConfiguration redisConfiguration, RabbitMqConfiguration rabbitMqConfiguration) =>
        x.AddSagaStateMachine<InfohashMetadataSagaStateMachine, InfohashMetadataSagaState>(
                cfg =>
                {
                    cfg.UseMessageRetry(r => r.Intervals(1000,2000,5000));
                    cfg.UseInMemoryOutbox();
                })
            .RedisRepository(redisConfiguration.ConnectionString, options =>
            {
                options.KeyPrefix = "debrid-collector:";
            })
            .Endpoint(
                e =>
                {
                    e.Name = rabbitMqConfiguration.DebridCollectorQueueName;
                    e.ConcurrentMessageLimit = 50;
                    e.PrefetchCount = 50;
                });
}
