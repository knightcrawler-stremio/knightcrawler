namespace Producer.Features.JobSupport;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddQuartz(this IServiceCollection services, IConfiguration configuration)
    {
        var scrapeConfiguration = services.LoadConfigurationFromConfig<ScrapeConfiguration>(configuration, ScrapeConfiguration.SectionName);
        var githubConfiguration = services.LoadConfigurationFromEnv<GithubConfiguration>();
        var rabbitConfiguration = services.LoadConfigurationFromEnv<RabbitMqConfiguration>();

        var jobTypes = Assembly.GetAssembly(typeof(BaseJob))
            .GetTypes()
            .Where(t => t is {IsClass: true, IsAbstract: false} && typeof(IJob).IsAssignableFrom(t) &&
            !Attribute.IsDefined(t, typeof(ManualJobRegistrationAttribute)))
            .ToList();

        foreach (var type in jobTypes)
        {
            services.AddTransient(type);
        }

        if (!string.IsNullOrEmpty(githubConfiguration.PAT))
        {
            services.AddTransient<SyncDmmJob>();
        }

        var openMethod = typeof(ServiceCollectionExtensions).GetMethod(nameof(AddJobWithTrigger), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        services.AddQuartz(
            quartz =>
            {
                //RegisterAutomaticRegistrationJobs(jobTypes, openMethod, quartz, scrapeConfiguration);
                RegisterDmmJob(githubConfiguration, quartz, scrapeConfiguration);
                //RegisterTorrentioJob(services, quartz, configuration, scrapeConfiguration);
                //RegisterPublisher(quartz, rabbitConfiguration);
            });

        services.AddQuartzHostedService(
            options =>
            {
                options.WaitForJobsToComplete = true;
            });

        return services;
    }

    private static void RegisterAutomaticRegistrationJobs(List<Type> jobTypes, MethodInfo? openMethod, IServiceCollectionQuartzConfigurator quartz,
        ScrapeConfiguration scrapeConfiguration)
    {
        foreach (var jobType in jobTypes)
        {
            var key = jobType.GetField("Key")?.GetValue(jobType);
            var trigger = jobType.GetField("Trigger")?.GetValue(jobType);

            if (key is null || trigger is null)
            {
                Console.WriteLine($"Job {jobType.Name} does not have a JobKey or TriggerKey property");
                continue;
            }

            var method = openMethod.MakeGenericMethod(jobType);
            method.Invoke(null, [quartz, key, trigger, scrapeConfiguration]);
        }
    }

    private static void RegisterDmmJob(GithubConfiguration githubConfiguration, IServiceCollectionQuartzConfigurator quartz, ScrapeConfiguration scrapeConfiguration)
    {
        if (!string.IsNullOrEmpty(githubConfiguration.PAT))
        {
            AddJobWithTrigger<SyncDmmJob>(quartz, SyncDmmJob.Key, SyncDmmJob.Trigger, scrapeConfiguration);
        }
    }

    private static void RegisterTorrentioJob(
        IServiceCollection services,
        IServiceCollectionQuartzConfigurator quartz,
        IConfiguration configuration,
        ScrapeConfiguration scrapeConfiguration)
    {
       var torrentioConfiguration = services.LoadConfigurationFromConfig<TorrentioConfiguration>(configuration, TorrentioConfiguration.SectionName);

       if (torrentioConfiguration.Instances.Count != 0)
       {
           AddJobWithTrigger<SyncTorrentioJob>(quartz, SyncTorrentioJob.Key, SyncTorrentioJob.Trigger, scrapeConfiguration);
       }
    }

    private static void RegisterPublisher(IServiceCollectionQuartzConfigurator quartz, RabbitMqConfiguration rabbitConfig) =>
        AddJobWithTriggerAndInterval<PublisherJob>(quartz, PublisherJob.Key, PublisherJob.Trigger, rabbitConfig.PublishIntervalInSeconds);

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

    private static void AddJobWithTriggerAndInterval<TJobType>(
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
