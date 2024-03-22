namespace Metadata.Features.DownloadImdbData;

public interface IImdbFileDownloader
{
    Task<string> DownloadFileToTempPath(HttpClient client, string fileName, CancellationToken cancellationToken);
}