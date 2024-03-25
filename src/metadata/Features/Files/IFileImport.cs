namespace Metadata.Features.Files;

public interface IFileImport<TImportType>
{
    Task Import(string fileName, int batchSize, CancellationToken cancellationToken);
}