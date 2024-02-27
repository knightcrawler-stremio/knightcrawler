namespace Metadata.Features.DeleteDownloadedImdbData;

public class DeleteDownloadedImdbDataRequestHandler(ILogger<DeleteDownloadedImdbDataRequestHandler> logger)
{
    public Task Handle(DeleteDownloadedImdbDataRequest request, CancellationToken _)
    {
        logger.LogInformation("Deleting file {FilePath}", request.FilePath);
        
        File.Delete(request.FilePath);
        
        logger.LogInformation("File Deleted");

        return Task.CompletedTask;
    }
}