namespace DebridCollector.Features.Worker;

[EntityName("perform-metadata-request-debrid-collector")]
public record PerformMetadataRequest(Guid CorrelationId, string InfoHash) : CorrelatedBy<Guid>;

[EntityName("torrent-metadata-response-debrid-collector")]
public record GotMetadata(TorrentMetadataResponse Metadata) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; } = Metadata.CorrelationId;
}

[EntityName("write-metadata-debrid-collector")]
public record WriteMetadata(Torrent Torrent, TorrentMetadataResponse Metadata, string ImdbId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; } = Metadata.CorrelationId;
}

[EntityName("metadata-written-debrid-colloctor")]
public record MetadataWritten(TorrentMetadataResponse Metadata, bool WithFiles) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; } = Metadata.CorrelationId;
}