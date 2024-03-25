namespace QBitCollector.Features.Worker;

public class PerformQbitMetadataRequestConsumer(QbitRequestProcessor processor) : IConsumer<PerformQbitMetadataRequest>
{
    public async Task Consume(ConsumeContext<PerformQbitMetadataRequest> context)
    {
        var request = context.Message;

        var metadata = await processor.ProcessAsync(request.InfoHash, context.CancellationToken);
        
        if (metadata is null)
        {
            var emptyresponse = new QBitMetadataResponse(request.CorrelationId, []);
            await context.Publish(new GotQbitMetadata(emptyresponse));
            return;
        }
        
        var response = new QBitMetadataResponse(request.CorrelationId, metadata);
        await context.Publish(new GotQbitMetadata(response));
    }
}