namespace DebridCollector.Features.Worker;

[EntityName("perform-metadata-request")]
public record PerformMetadataRequest(Guid CorrelationId, string InfoHash) : CorrelatedBy<Guid>;

[EntityName("torrent-metadata-response")]
public record GotMetadata(TorrentMetadataResponse Metadata) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; } = Metadata.CorrelationId;
}

[EntityName("write-metadata")]
public record WriteMetadata(Torrent Torrent, TorrentMetadataResponse Metadata, string ImdbId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; } = Metadata.CorrelationId;
}

[EntityName("metadata-written")]
public record MetadataWritten(TorrentMetadataResponse Metadata) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; } = Metadata.CorrelationId;
}