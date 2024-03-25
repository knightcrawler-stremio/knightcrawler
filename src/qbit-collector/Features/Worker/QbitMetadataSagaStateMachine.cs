namespace QBitCollector.Features.Worker;

public class QbitMetadataSagaStateMachine : MassTransitStateMachine<QbitMetadataSagaState>
{
    public State Ingesting { get; private set; } = null!;
    public State Writing { get; private set; } = null!;
    public State Completed { get; private set; } = null!;

    public Event<CollectMetadata> CollectMetadata { get; private set; } = null!;
    public Event<GotQbitMetadata> GotMetadata { get; private set; } = null!;
    public Event<QbitMetadataWritten> MetadataWritten { get; private set; } = null!;

    public QbitMetadataSagaStateMachine(ILogger<QbitMetadataSagaStateMachine> logger)
    {
        InstanceState(x => x.CurrentState);

        Event(() => CollectMetadata, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => GotMetadata, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => MetadataWritten, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(CollectMetadata)
                .ThenAsync(
                    async context =>
                    {
                        context.Saga.CorrelationId = context.Data.CorrelationId;
                        context.Saga.Torrent = context.Data.Torrent;
                        context.Saga.ImdbId = context.Data.ImdbId;

                        await context.Publish(new PerformQbitMetadataRequest(context.Saga.CorrelationId, context.Saga.Torrent.InfoHash));
                        
                        logger.LogInformation("Collecting Metadata for torrent {InfoHash} in Saga {SagaId}", context.Instance.Torrent.InfoHash, context.Instance.CorrelationId);
                    })
                .TransitionTo(Ingesting));
        
        During(
            Ingesting,
            When(GotMetadata)
                .ThenAsync(
                    async context =>
                    {
                        context.Saga.Metadata = context.Data.Metadata;
                        
                        await context.Publish(new WriteQbitMetadata(context.Saga.Torrent, context.Saga.Metadata, context.Saga.ImdbId));
                        
                        logger.LogInformation("Scheduling Write for torrent {InfoHash} in Saga {SagaId}", context.Saga.Torrent.InfoHash, context.Saga.CorrelationId);
                    })
                .TransitionTo(Writing));
        
        During(
            Writing,
            When(MetadataWritten)
                .Then(
                    context =>
                    {
                        logger.LogInformation("Metadata Written for torrent {InfoHash} in Saga {SagaId}", context.Saga.Torrent.InfoHash, context.Saga.CorrelationId);
                    })
                .TransitionTo(Completed)
                .Finalize());

        SetCompletedWhenFinalized();
    }
}
