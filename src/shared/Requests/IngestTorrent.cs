namespace SharedContracts.Requests;

public record IngestTorrent(Guid CorrelationId, IngestedTorrent IngestedTorrent) : CorrelatedBy<Guid>;