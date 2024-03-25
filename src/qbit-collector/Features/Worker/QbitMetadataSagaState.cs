namespace QBitCollector.Features.Worker;

public class QbitMetadataSagaState : SagaStateMachineInstance, ISagaVersion
{
    public Torrent? Torrent { get; set; }
    public string? Title { get; set; }
    public string? ImdbId { get; set; }
    public QBitMetadataResponse? Metadata { get; set; }
    
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public int CurrentState { get; set; }
}