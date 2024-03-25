namespace SharedContracts.Requests;

public record CollectMetadata(Guid CorrelationId, Torrent Torrent, string ImdbId) : CorrelatedBy<Guid>;