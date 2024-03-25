namespace QBitCollector.Features.Worker;

[EntityName("perform-metadata-request")]
public record PerformQbitMetadataRequest(Guid CorrelationId, string InfoHash) : CorrelatedBy<Guid>;

[EntityName("torrent-metadata-response")]
public record GotQbitMetadata(QBitMetadataResponse Metadata) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; } = Metadata.CorrelationId;
}

[EntityName("write-metadata")]
public record WriteQbitMetadata(Torrent Torrent, QBitMetadataResponse Metadata, string ImdbId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; } = Metadata.CorrelationId;
}

[EntityName("metadata-written")]
public record QbitMetadataWritten(QBitMetadataResponse Metadata) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; } = Metadata.CorrelationId;
}