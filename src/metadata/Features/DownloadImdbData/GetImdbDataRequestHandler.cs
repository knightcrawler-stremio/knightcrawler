namespace Metadata.Features.DownloadImdbData;

public class GetImdbDataRequestHandler(IHttpClientFactory clientFactory, ILogger<GetImdbDataRequestHandler> logger)
{
    private const string TitleBasicsFileName = "title.basics.tsv";
    private const string TitleAkasFileName = "title.akas.tsv";
    private const string EpisodesFileName = "title.episode.tsv";

    public async Task<ImportImdbDataRequest> Handle(GetImdbDataRequest _, CancellationToken cancellationToken)
    {
        logger.LogInformation("Downloading IMDB data");

        var client = clientFactory.CreateClient("imdb-data");
        var tempBasicsFile = await DownloadFileToTempPath(client, TitleBasicsFileName, cancellationToken);
        var tempAkasFile = await DownloadFileToTempPath(client, TitleAkasFileName, cancellationToken);
        var tempEpisodesFile = await DownloadFileToTempPath(client, EpisodesFileName, cancellationToken);

        return new(tempBasicsFile, tempAkasFile, tempEpisodesFile);
    }

    private async Task<string> DownloadFileToTempPath(HttpClient client, string fileName, CancellationToken cancellationToken)
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
