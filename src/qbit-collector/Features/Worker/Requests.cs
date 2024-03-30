namespace QBitCollector.Features.Worker;

[EntityName("perform-metadata-request-qbit-collector")]
public record PerformQbitMetadataRequest(Guid CorrelationId, string InfoHash) : CorrelatedBy<Guid>;

[EntityName("torrent-metadata-response-qbit-collector")]
public record GotQbitMetadata(QBitMetadataResponse Metadata) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; } = Metadata.CorrelationId;
}

[EntityName("write-metadata-qbit-collector")]
public record WriteQbitMetadata(Torrent Torrent, QBitMetadataResponse Metadata, string ImdbId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; } = Metadata.CorrelationId;
}

[EntityName("metadata-written-qbit-collector")]
public record QbitMetadataWritten(QBitMetadataResponse Metadata) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; } = Metadata.CorrelationId;

    public QBitMetadataResponse Metadata { get; init; } = Metadata;
}