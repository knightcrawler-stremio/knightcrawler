namespace Metadata.Features.DownloadImdbData;

public class GetImdbDataRequestHandler(IHttpClientFactory clientFactory, IImdbFileDownloader downloader, ILogger<GetImdbDataRequestHandler> logger)
{
    private const string TitleBasicsFileName = "title.basics.tsv";
    private const string TitleAkasFileName = "title.akas.tsv";
    private const string EpisodesFileName = "title.episode.tsv";

    public async Task<ClearExistingImdbDataRequest> Handle(GetImdbDataRequest _, CancellationToken cancellationToken)
    {
        logger.LogInformation("Downloading IMDB data");

        var client = clientFactory.CreateClient("imdb-data");
        var tempBasicsFile = await downloader.DownloadFileToTempPath(client, TitleBasicsFileName, cancellationToken);
        var tempAkasFile = await downloader.DownloadFileToTempPath(client, TitleAkasFileName, cancellationToken);
        var tempEpisodesFile = await downloader.DownloadFileToTempPath(client, EpisodesFileName, cancellationToken);

        return new(tempBasicsFile, tempAkasFile, tempEpisodesFile);
    }
}
