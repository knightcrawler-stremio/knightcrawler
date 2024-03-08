namespace Metadata.Features.Jobs;

public class JobScheduler(IServiceProvider serviceProvider) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();

        var mongoDbService = scope.ServiceProvider.GetRequiredService<ImdbMongoDbService>();

        if (!mongoDbService.IsDatabaseInitialized())
        {
            throw new InvalidOperationException("MongoDb is not initialized");
        }

        var jobConfigurations = scope.ServiceProvider.GetRequiredService<JobConfiguration>();
        var downloadJob = scope.ServiceProvider.GetRequiredService<DownloadImdbDataJob>();

        if (!downloadJob.IsScheduelable)
        {
            return downloadJob.Invoke();
        }

        var scheduler = scope.ServiceProvider.GetRequiredService<IScheduler>();

        scheduler.Schedule<DownloadImdbDataJob>()
            .Cron(jobConfigurations.DownloadImdbCronSchedule)
            .PreventOverlapping(nameof(downloadJob.JobName));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
