namespace DebridCollector.Features.Worker;

public class PerformMetadataRequestConsumer : IConsumer<PerformMetadataRequest>
{
    public Task Consume(ConsumeContext<PerformMetadataRequest> context)
    {
        ProcessorChannel.AddToQueue(context.Message);
        return Task.CompletedTask;
    }
}