namespace Metadata.Features.DownloadImdbData;

public class ImdbFileDownloader(ILogger<ImdbFileDownloader> logger) : IImdbFileDownloader
{
    public async Task<string> DownloadFileToTempPath(HttpClient client, string fileName, CancellationToken cancellationToken)
    {
        var response = await client.GetAsync($"{fileName}.gz", cancellationToken);

        var tempFile = Path.Combine(Path.GetTempPath(), fileName);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
        await using var fileStream = File.Create(tempFile);

        await gzipStream.CopyToAsync(fileStream, cancellationToken);

        logger.LogInformation("Downloaded IMDB data '{Filename}' to {TempFile}", fileName, tempFile);

        fileStream.Close();
        return tempFile;
    }
}