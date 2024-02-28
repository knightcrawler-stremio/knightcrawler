namespace Metadata.Features.DownloadImdbData;

public class GetImdbDataRequestHandler(IHttpClientFactory clientFactory, ILogger<GetImdbDataRequestHandler> logger)
{
    private const string TitleBasicsFileName = "title.basics.tsv";
    
    public async Task<ImportImdbDataRequest> Handle(GetImdbDataRequest _, CancellationToken cancellationToken)
    {
        logger.LogInformation("Downloading IMDB data");
        
        var client = clientFactory.CreateClient("imdb-data");
        var response = await client.GetAsync($"{TitleBasicsFileName}.gz", cancellationToken);
        
        var tempFile = Path.Combine(Path.GetTempPath(), TitleBasicsFileName);
        
        response.EnsureSuccessStatusCode();
        
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
        await using var fileStream = File.Create(tempFile);
        
        await gzipStream.CopyToAsync(fileStream, cancellationToken);
        
        logger.LogInformation("Downloaded IMDB data to {TempFile}", tempFile);
        
        fileStream.Close();

        return new(tempFile);
    }
}