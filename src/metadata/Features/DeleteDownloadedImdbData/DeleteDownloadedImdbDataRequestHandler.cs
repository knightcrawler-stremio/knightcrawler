namespace Metadata.Features.DeleteDownloadedImdbData;

public class DeleteDownloadedImdbDataRequestHandler(ILogger<DeleteDownloadedImdbDataRequestHandler> logger)
{
    public Task Handle(DeleteDownloadedImdbDataRequest request, CancellationToken _)
    {
        DeleteFile(request.TitleBasicsFilePath);
        DeleteFile(request.TitleAkasFilePath);
        DeleteFile(request.EpisodesFilePath);
        logger.LogInformation("Processing Completed");

        Environment.Exit(0);

        return Task.CompletedTask;
    }

    private void DeleteFile(string file)
    {
        logger.LogInformation("Deleting file {FilePath}", file);
        File.Delete(file);
        logger.LogInformation("File Deleted");
    }
}
