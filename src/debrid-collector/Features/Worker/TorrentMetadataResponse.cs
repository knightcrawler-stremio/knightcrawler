namespace DebridCollector.Features.Worker;

[EntityName("torrent-metadata-response")]
public record TorrentMetadataResponse(Guid CorrelationId, FileDataDictionary Metadata) : CorrelatedBy<Guid>;