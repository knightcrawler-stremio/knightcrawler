using Producer.Models.Configuration;

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
        services.LoadConfigurationFromEnv<PostgresConfiguration>();
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
            busConfigurator.UsingRabbitMq((_, busFactoryConfigurator) =>
            {
                busFactoryConfigurator.Host(rabbitConfig.Host, hostConfigurator =>
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
        var scrapeConfiguration = services.LoadConfigurationFromConfig<ScrapeConfiguration>(configuration, ScrapeConfiguration.SectionName);
        var githubConfiguration = services.LoadConfigurationFromConfig<GithubConfiguration>(configuration, GithubConfiguration.SectionName);
        var rabbitConfig = services.LoadConfigurationFromConfig<RabbitMqConfiguration>(configuration, RabbitMqConfiguration.SectionName);

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
    
    private static TConfiguration LoadConfigurationFromConfig<TConfiguration>(this IServiceCollection services, IConfiguration configuration, string sectionName)
        where TConfiguration : class
    {
        var instance = configuration.GetSection(sectionName).Get<TConfiguration>();
        
        ArgumentNullException.ThrowIfNull(instance, nameof(instance));

        services.TryAddSingleton(instance);

        return instance;
    }
    
    private static TConfiguration LoadConfigurationFromEnv<TConfiguration>(this IServiceCollection services)
        where TConfiguration : class
    {
        var instance = Activator.CreateInstance<TConfiguration>();
        
        ArgumentNullException.ThrowIfNull(instance, nameof(instance));

        services.TryAddSingleton(instance);

        return instance;
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