namespace TorrentConsumer.Features.TorrentIngestion;

[EntityName("perform-ingestion-actions")]
public record PerformIngestionActions(Guid CorrelationId, IngestedTorrent IngestedTorrent) : CorrelatedBy<Guid>;

[EntityName("ingestion-completed")]
public record IngestionCompleted(Guid CorrelationId, Torrent Torrent) : CorrelatedBy<Guid>;