namespace QBitCollector.Extensions;

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
        services.AddQBitTorrentClient();
        services.RegisterPythonEngine();
        services.AddSingleton<IRankTorrentName, RankTorrentName>();
        services.AddSingleton<QbitRequestProcessor>();
        services.AddHttpClient();
        services.AddSingleton<ITrackersService, TrackersService>();
        services.AddHostedService<TrackersBackgroundService>();
        services.AddHostedService<HousekeepingBackgroundService>();

        return services;
    }

    internal static IServiceCollection AddRedisCache(this IServiceCollection services)
    {
        var redisConfiguration = services.LoadConfigurationFromEnv<RedisConfiguration>();

        services.AddStackExchangeRedisCache(
            option =>
            {
                option.InstanceName = "qbit-collector:";
                option.Configuration = redisConfiguration.ConnectionString;
            });

        services.AddMemoryCache();

        return services;
    }

    internal static IServiceCollection RegisterMassTransit(this IServiceCollection services)
    {
        var rabbitConfiguration = services.LoadConfigurationFromEnv<RabbitMqConfiguration>();
        var redisConfiguration = services.LoadConfigurationFromEnv<RedisConfiguration>();
        var qbitConfiguration = services.LoadConfigurationFromEnv<QbitConfiguration>();

        services.AddStackExchangeRedisCache(
            option =>
            {
                option.InstanceName = "qbit-collector:";
                option.Configuration = redisConfiguration.ConnectionString;
            });
        
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            
            x.AddConsumer<WriteQbitMetadataConsumer>();

            x.AddConsumer<PerformQbitMetadataRequestConsumer>();
            
            x.RegisterMetadataIngestionSaga(redisConfiguration);
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.AutoStart = true;

                cfg.Host(
                    rabbitConfiguration.Host, h =>
                    {
                        h.Username(rabbitConfiguration.Username);
                        h.Password(rabbitConfiguration.Password);
                    });

                cfg.Message<CollectMetadata>(e => e.SetEntityName(rabbitConfiguration.QbitCollectorQueueName));
                
                cfg.ReceiveEndpoint(rabbitConfiguration.QbitCollectorQueueName, e =>
                {
                    e.ConfigureConsumer<WriteQbitMetadataConsumer>(context);
                    e.ConfigureConsumer<PerformQbitMetadataRequestConsumer>(context);
                    e.ConfigureSaga<QbitMetadataSagaState>(context);
                    e.ConcurrentMessageLimit = qbitConfiguration.Concurrency;
                    e.PrefetchCount = qbitConfiguration.Concurrency;
                });
            });
        });
        
        return services;
    }

    private static void RegisterMetadataIngestionSaga(this IBusRegistrationConfigurator x, RedisConfiguration redisConfiguration) =>
        x.AddSagaStateMachine<QbitMetadataSagaStateMachine, QbitMetadataSagaState>(
                cfg =>
                {
                    cfg.UseMessageRetry(r => r.Intervals(1000, 2000, 5000));
                    cfg.UseInMemoryOutbox();
                    cfg.UseTimeout(
                        timeout =>
                        {
                            timeout.Timeout = TimeSpan.FromMinutes(3);
                        });
                })
            .RedisRepository(redisConfiguration.ConnectionString, options =>
            {
                options.KeyPrefix = "qbit-collector:";
            });
    
    private static void AddQBitTorrentClient(this IServiceCollection services)
    {
        var qbitConfiguration = services.LoadConfigurationFromEnv<QbitConfiguration>();
        var client = new QBittorrentClient(new(qbitConfiguration.Host));
        client.Timeout = TimeSpan.FromSeconds(20);

        services.AddSingleton<IQBittorrentClient>(client);
    }
}
