namespace DebridCollector.Features.Debrid;

public static class ProcessorChannel
{
    public static Channel<PerformMetadataRequest> Queue = Channel.CreateUnbounded<PerformMetadataRequest>(new()
    {
        SingleReader = true,
        SingleWriter = true,
    });
    
    public static bool AddToQueue(PerformMetadataRequest infoHash) => Queue.Writer.TryWrite(infoHash);
}