namespace Metadata.Features.IndexImdbData;

public class IndexImdbDataRequestHandler(ILogger<IndexImdbDataRequestHandler> logger, ImdbDbService dbService)
{
    public async Task<DeleteDownloadedImdbDataRequest> Handle(IndexImdbDataRequest request, CancellationToken _)
    {
        logger.LogInformation("Creating Trigram Indexes for IMDB data");

        await dbService.CreatePgtrmIndex();

        return new(request.FilePath);
    }
}
