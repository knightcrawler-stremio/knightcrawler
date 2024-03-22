namespace Metadata.Features.ImportImdbData;

public class ImportImdbDataRequestHandler(
    ImdbDbService dbService,
    ILogger<ImportImdbDataRequestHandler> logger,
    ServiceConfiguration configuration,
    IFileImport<ImdbBasicEntry> basicsFile,
    IFileImport<ImdbAkaEntry> akasFile,
    IFileImport<ImdbEpisodeEntry> episodesFile)
{
    public async Task<IndexImdbDataRequest> Handle(ImportImdbDataRequest request, CancellationToken cancellationToken)
    {
        await PreImportClearDb();

        await basicsFile.Import(request.TitleBasicsFilePath, configuration.InsertBatchSize, cancellationToken);
        await akasFile.Import(request.TitleAkasFilePath, configuration.InsertBatchSize, cancellationToken);
        await episodesFile.Import(request.EpisodesFilePath, configuration.InsertBatchSize, cancellationToken);
        
        return new(request.TitleBasicsFilePath, request.TitleAkasFilePath, request.EpisodesFilePath);
    }

    private async Task PreImportClearDb()
    {
        logger.LogInformation("Clearing existing IMDB data from database");
        await dbService.DropPgtrmIndex();
        await dbService.DeleteFromTable(TableNames.MetadataTable);
        logger.LogInformation("Existing IMDB data cleared from database");
    }
}