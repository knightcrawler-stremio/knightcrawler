namespace Producer.Features.Crawlers.Dmm;

public interface IDMMFileDownloader
{
    Task<string> DownloadFileToTempPath(CancellationToken cancellationToken);
}