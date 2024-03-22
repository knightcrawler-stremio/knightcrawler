using Metadata.Features.IndexImdbData;

namespace Metadata.Features.ImportImdbData;

public class ImportImdbDataRequestHandler(ILogger<ImportImdbDataRequestHandler> logger, ImdbDbService dbService, ServiceConfiguration configuration)
{
    public async Task<IndexImdbDataRequest> Handle(ImportImdbDataRequest request, CancellationToken cancellationToken)
    {
        await BasicsFile.Import(request.TitleBasicsFilePath, logger, dbService, configuration.InsertBatchSize, cancellationToken);
        await AkasFile.Import(request.TitleAkasFilePath, logger, dbService, configuration.InsertBatchSize, cancellationToken);

        return new(request.TitleBasicsFilePath, request.TitleAkasFilePath);
    }
}