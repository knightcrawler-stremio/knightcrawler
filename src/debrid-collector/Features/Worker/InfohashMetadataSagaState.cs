namespace DebridCollector.Features.Worker;

public class InfohashMetadataSagaState : SagaStateMachineInstance, ISagaVersion
{
    public Torrent? Torrent { get; set; }
    public string? Title { get; set; }
    public string? ImdbId { get; set; }
    public TorrentMetadataResponse? Metadata { get; set; }
    public int RetriesAllowed { get; set; } = 2;
    
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public int CurrentState { get; set; }
}