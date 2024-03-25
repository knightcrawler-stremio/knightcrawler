namespace QBitCollector.Features.Trackers;

public class TrackersBackgroundService(ITrackersService trackersService) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => trackersService.GetTrackers();
    
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}