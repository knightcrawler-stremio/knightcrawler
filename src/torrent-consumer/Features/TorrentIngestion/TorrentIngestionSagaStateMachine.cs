namespace TorrentConsumer.Features.TorrentIngestion;

// ReSharper disable UnassignedGetOnlyAutoProperty MemberCanBePrivate.Global
public class TorrentIngestionSagaStateMachine : MassTransitStateMachine<TorrentIngestionState>
{
    public State Ingesting { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public Event<IngestTorrent> IngestTorrentEvent { get; private set; } = null!;
    public Event<IngestionCompleted> IngestionCompletedEvent { get; private set; } = null!;

    public TorrentIngestionSagaStateMachine(ILogger<TorrentIngestionSagaStateMachine> logger)
    {
        InstanceState(x => x.CurrentState);

        Event(() => IngestTorrentEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => IngestionCompletedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(IngestTorrentEvent)
                .ThenAsync(async context =>
                {
                    context.Saga.CorrelationId = context.Data.CorrelationId;
                    context.Saga.IngestedTorrent = context.Data.IngestedTorrent;
                    
                    var performIngestionActions = new PerformIngestionActions(context.Instance.CorrelationId, context.Instance.IngestedTorrent);
                    await context.Publish(performIngestionActions);
                    
                    logger.LogInformation("Ingesting torrent {InfoHash} in Saga {SagaId}", context.Instance.IngestedTorrent.InfoHash, context.Instance.CorrelationId);
                })
                .TransitionTo(Ingesting));

        During(Ingesting,
            When(IngestionCompletedEvent)
                .ThenAsync(async context =>
                {
                    context.Saga.Torrent = context.Data.Torrent;
                    await context.Publish(new CollectMetadata(Guid.NewGuid(), context.Saga.Torrent, context.Saga.IngestedTorrent.Imdb));
                    
                    logger.LogInformation("Ingestion completed for torrent {InfoHash} in Saga {SagaId}", context.Saga.IngestedTorrent.InfoHash, context.Saga.CorrelationId);
                })
                .Finalize());

        SetCompletedWhenFinalized();
    }
}