namespace Metadata.Features.DeleteDownloadedImdbData;

public class DeleteDownloadedImdbDataRequestHandler(ILogger<DeleteDownloadedImdbDataRequestHandler> logger, JobConfiguration configuration)
{
    public Task Handle(DeleteDownloadedImdbDataRequest request, CancellationToken _)
    {
        logger.LogInformation("Deleting file {FilePath}", request.FilePath);
        
        File.Delete(request.FilePath);
        
        logger.LogInformation("File Deleted");

        if (configuration.DownloadImdbOnce)
        {
            logger.LogInformation("Processing Completed: Exiting application as DownloadImdbOnce is set to true");
            Environment.Exit(0);
        }

        return Task.CompletedTask;
    }
}