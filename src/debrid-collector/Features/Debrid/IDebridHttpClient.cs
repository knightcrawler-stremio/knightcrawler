namespace DebridCollector.Features.Debrid;

public interface IDebridHttpClient
{
    public Task<IReadOnlyList<TorrentMetadataResponse>> GetMetadataAsync(IReadOnlyCollection<PerformMetadataRequest> infoHashes, CancellationToken cancellationToken = default);   
}