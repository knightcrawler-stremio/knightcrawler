namespace Producer.Extensions;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddCrawlers(this IServiceCollection services)
    {
        services.AddHttpClient("Scraper");

        services
            .AddKeyedTransient<ICrawler, EzTvCrawler>(nameof(EzTvCrawler))
            .AddKeyedTransient<ICrawler, YtsCrawler>(nameof(YtsCrawler))
            .AddKeyedTransient<ICrawler, TpbCrawler>(nameof(TpbCrawler))
            .AddKeyedTransient<ICrawler, TgxCrawler>(nameof(TgxCrawler))
            .AddKeyedTransient<ICrawler, DebridMediaManagerCrawler>(nameof(DebridMediaManagerCrawler))
            .AddSingleton<ICrawlerProvider, CrawlerProvider>()
            .AddTransient<IIpService, IpService>();

        return services;
    }
    
    internal static IServiceCollection AddDataStorage(this IServiceCollection services)
    {
        services.AddTransient<IDataStorage, DapperDataStorage>();
        services.AddTransient<IMessagePublisher, TorrentPublisher>();
        return services;
    }

    internal static IServiceCollection RegisterMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitConfig = configuration.GetSection(RabbitMqConfiguration.SectionName).Get<RabbitMqConfiguration>();
        
        ArgumentNullException.ThrowIfNull(rabbitConfig, nameof(rabbitConfig));

        services.AddSingleton(rabbitConfig);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingRabbitMq((context, busFactoryConfigurator) =>
            {
                busFactoryConfigurator.Host(rabbitConfig!.Host, hostConfigurator =>
                {
                    hostConfigurator.Username(rabbitConfig.Username);
                    hostConfigurator.Password(rabbitConfig.Password);
                });
            });
        });

        return services;
    }
    
    internal static IServiceCollection AddQuartz(this IServiceCollection services, IConfiguration configuration)
    {
        var scrapeConfiguration = LoadScrapeConfiguration(services, configuration);
        var githubConfiguration = LoadGithubConfiguration(services, configuration);
        var rabbitConfig = LoadRabbitMQConfiguration(services, configuration);

        services
            .AddTransient<SyncEzTvJob>()
            .AddTransient<SyncTpbJob>()
            .AddTransient<SyncYtsJob>()
            .AddTransient<SyncTgxJob>()
            .AddTransient<IPJob>()
            .AddTransient<PublisherJob>();

        if (!string.IsNullOrEmpty(githubConfiguration.PAT))
        {
            services.AddTransient<SyncDmmJob>();
        }

        services.AddQuartz(
            quartz =>
            {
               AddJobWithTrigger<SyncEzTvJob>(quartz, SyncEzTvJob.Key, SyncEzTvJob.Trigger, scrapeConfiguration);
               AddJobWithTrigger<SyncTpbJob>(quartz, SyncTpbJob.Key, SyncTpbJob.Trigger, scrapeConfiguration);
               AddJobWithTrigger<SyncYtsJob>(quartz, SyncYtsJob.Key, SyncYtsJob.Trigger, scrapeConfiguration);
               AddJobWithTrigger<SyncTgxJob>(quartz, SyncTgxJob.Key, SyncTgxJob.Trigger, scrapeConfiguration);
               AddJobWithTrigger<IPJob>(quartz, IPJob.Key, IPJob.Trigger, 60 * 5);
               AddJobWithTrigger<PublisherJob>(quartz, PublisherJob.Key, PublisherJob.Trigger, rabbitConfig.PublishIntervalInSeconds);

                if (!string.IsNullOrEmpty(githubConfiguration.PAT))
                {
                     AddJobWithTrigger<SyncDmmJob>(quartz, SyncDmmJob.Key, SyncDmmJob.Trigger, scrapeConfiguration);
                }
            });

        services.AddQuartzHostedService(
            options =>
            {
                options.WaitForJobsToComplete = true;
            });

        return services;
    }

    private static GithubConfiguration LoadGithubConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        var githubConfiguration = configuration.GetSection(GithubConfiguration.SectionName).Get<GithubConfiguration>();
        
        ArgumentNullException.ThrowIfNull(githubConfiguration, nameof(githubConfiguration));
        
        services.TryAddSingleton(githubConfiguration);

        return githubConfiguration;
    }

    private static RabbitMqConfiguration LoadRabbitMQConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitConfiguration = configuration.GetSection(RabbitMqConfiguration.SectionName).Get<RabbitMqConfiguration>();

        ArgumentNullException.ThrowIfNull(rabbitConfiguration, nameof(rabbitConfiguration));

        if (rabbitConfiguration.MaxQueueSize > 0)
        {
            if (rabbitConfiguration.MaxPublishBatchSize > rabbitConfiguration.MaxQueueSize)
            {
                throw new InvalidOperationException("MaxPublishBatchSize cannot be greater than MaxQueueSize in RabbitMqConfiguration");
            }
        }

        services.TryAddSingleton(rabbitConfiguration);

        return rabbitConfiguration;
    }

    private static ScrapeConfiguration LoadScrapeConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        var scrapeConfiguration = configuration.GetSection(ScrapeConfiguration.SectionName).Get<ScrapeConfiguration>();
        
        ArgumentNullException.ThrowIfNull(scrapeConfiguration, nameof(scrapeConfiguration));

        services.TryAddSingleton(scrapeConfiguration);

        return scrapeConfiguration;
    }

    private static void AddJobWithTrigger<TJobType>(
        IServiceCollectionQuartzConfigurator quartz,
        JobKey key,
        TriggerKey trigger,
        ScrapeConfiguration scrapeConfiguration) where TJobType : IJob
    {
        var scraper = scrapeConfiguration.Scrapers
            .FirstOrDefault(x => x.Name != null && 
                                 x.Name.Equals(typeof(TJobType).Name, StringComparison.OrdinalIgnoreCase));

        if (scraper is null || !scraper.Enabled)
        {
            return;
        }
        
        quartz.AddJob<TJobType>(opts => opts.WithIdentity(key).StoreDurably());

        quartz.AddTrigger(
            opts => opts
                .ForJob(key)
                .WithIdentity(trigger)
                .StartAt(DateTimeOffset.Now.AddSeconds(20))
                .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(scraper.IntervalSeconds)).RepeatForever()));
    }
    
    private static void AddJobWithTrigger<TJobType>(
        IServiceCollectionQuartzConfigurator quartz,
        JobKey key,
        TriggerKey trigger,
        int interval) where TJobType : IJob
    {
        quartz.AddJob<TJobType>(opts => opts.WithIdentity(key).StoreDurably());

        quartz.AddTrigger(
            opts => opts
                .ForJob(key)
                .WithIdentity(trigger)
                .StartAt(DateTimeOffset.Now.AddSeconds(20))
                .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(interval)).RepeatForever()));
    }
}