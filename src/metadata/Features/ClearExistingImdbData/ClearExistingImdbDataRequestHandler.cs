namespace Metadata.Features.ClearExistingImdbData;

public class ClearExistingImdbDataRequestHandler(ILogger<ClearExistingImdbDataRequestHandler> logger, ImdbDbService dbService)
{
    public async Task<ImportImdbDataRequest> Handle(ClearExistingImdbDataRequest request, CancellationToken _)
    {
        logger.LogInformation("Clearing existing IMDB data from database");
        await dbService.DropPgtrmIndex();
        await dbService.TruncateTable(TableNames.EpisodesTable);
        await dbService.TruncateTable(TableNames.AkasTable);
        await dbService.TruncateTable(TableNames.MetadataTable, cascade: true);
        logger.LogInformation("Existing IMDB data cleared from database");

        return new(request.TitleBasicsFilePath, request.TitleAkasFilePath, request.EpisodesFilePath);
    }
}
