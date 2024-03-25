namespace QBitCollector.Features.Worker;

[EntityName("torrent-metadata-response")]
public record QBitMetadataResponse(Guid CorrelationId, IReadOnlyList<TorrentContent> Metadata) : CorrelatedBy<Guid>;