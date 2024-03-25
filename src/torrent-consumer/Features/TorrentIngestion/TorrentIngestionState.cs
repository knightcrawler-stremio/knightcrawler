namespace TorrentConsumer.Features.TorrentIngestion;

public class TorrentIngestionState : SagaStateMachineInstance, ISagaVersion
{
    public int CurrentState { get; set; }
    public IngestedTorrent? IngestedTorrent { get; set; }
    public Torrent? Torrent { get; set; }
    
    public Guid CorrelationId { get; set; }

    public int Version { get; set; }
}