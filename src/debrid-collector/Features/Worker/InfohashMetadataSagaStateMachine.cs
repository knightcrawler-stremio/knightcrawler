namespace DebridCollector.Features.Worker;

public class InfohashMetadataSagaStateMachine : MassTransitStateMachine<InfohashMetadataSagaState>
{
    public State Ingesting { get; private set; } = null!;
    public State Writing { get; private set; } = null!;
    public State Completed { get; private set; } = null!;

    public Event<CollectMetadata> CollectMetadata { get; private set; } = null!;
    public Event<GotMetadata> GotMetadata { get; private set; } = null!;
    public Event<MetadataWritten> MetadataWritten { get; private set; } = null!;

    public InfohashMetadataSagaStateMachine(ILogger<InfohashMetadataSagaStateMachine> logger)
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

                        await context.Publish(new PerformMetadataRequest(context.Saga.CorrelationId, context.Saga.Torrent.InfoHash));
                        
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
                        
                        await context.Publish(new WriteMetadata(context.Saga.Torrent, context.Saga.Metadata, context.Saga.ImdbId));
                        
                        logger.LogInformation("Got Metadata for torrent {InfoHash} in Saga {SagaId}", context.Saga.Torrent.InfoHash, context.Saga.CorrelationId);
                    })
                .TransitionTo(Writing));
        
        During(
            Writing,
            When(MetadataWritten)
                .Then(
                    context =>
                    {
                        if (!context.Message.WithFiles)
                        {
                            logger.LogInformation("No files written for torrent {InfoHash} in Saga {SagaId}", context.Saga.Torrent.InfoHash, context.Saga.CorrelationId);
                            return;
                        }
                        
                        logger.LogInformation("Metadata Written for torrent {InfoHash} in Saga {SagaId}", context.Saga.Torrent.InfoHash, context.Saga.CorrelationId);
                    })
                .TransitionTo(Completed)
                .Finalize());

        SetCompletedWhenFinalized();
    }
}
