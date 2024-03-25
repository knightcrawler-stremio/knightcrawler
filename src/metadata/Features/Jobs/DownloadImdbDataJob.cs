namespace Metadata.Features.Jobs;

public class DownloadImdbDataJob(IMessageBus messageBus) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken) => 
        await messageBus.SendAsync(new GetImdbDataRequest());

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
