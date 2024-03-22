namespace Metadata.Features.ImportImdbData;

public class ImportImdbDataRequestHandler(
    ServiceConfiguration configuration,
    IFileImport<ImdbBasicEntry> basicsFile,
    IFileImport<ImdbAkaEntry> akasFile,
    IFileImport<ImdbEpisodeEntry> episodesFile)
{
    public async Task<IndexImdbDataRequest> Handle(ImportImdbDataRequest request, CancellationToken cancellationToken)
    {
        await basicsFile.Import(request.TitleBasicsFilePath, configuration.InsertBatchSize, cancellationToken);
        await akasFile.Import(request.TitleAkasFilePath, configuration.InsertBatchSize, cancellationToken);
        await episodesFile.Import(request.EpisodesFilePath, configuration.InsertBatchSize, cancellationToken);
        
        return new(request.TitleBasicsFilePath, request.TitleAkasFilePath, request.EpisodesFilePath);
    }
}