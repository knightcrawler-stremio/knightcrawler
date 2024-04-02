namespace Producer.Features.Crawlers.Dmm;

public class DMMFileDownloader(HttpClient client, ILogger<DMMFileDownloader> logger) : IDMMFileDownloader
{
    private const string Filename = "main.zip";
    private readonly IReadOnlyCollection<string> _filesToIgnore = [
        "index.html",
        "404.html",
        "dedupe.sh",
        "CNAME",
    ];
    
    public const string ClientName = "DmmFileDownloader";
    
    public async Task<string> DownloadFileToTempPath(CancellationToken cancellationToken)
    {
        logger.LogInformation("Downloading DMM Hashlists");
        
        var response = await client.GetAsync(Filename, cancellationToken);

        var tempDirectory = Path.Combine(Path.GetTempPath(), "DMMHashlists");

        EnsureDirectoryIsClean(tempDirectory);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var archive = new ZipArchive(stream);
        
        logger.LogInformation("Extracting DMM Hashlists to {TempDirectory}", tempDirectory);

        foreach (var entry in archive.Entries)
        {
            var entryPath = Path.Combine(tempDirectory, Path.GetFileName(entry.FullName));
            if (!entry.FullName.EndsWith('/')) // It's a file
            {
                entry.ExtractToFile(entryPath, true);
            }
        }

        foreach (var file in _filesToIgnore)
        {
            CleanRepoExtras(tempDirectory, file);    
        }
        
        logger.LogInformation("Downloaded and extracted Repository to {TempDirectory}", tempDirectory);

        return tempDirectory;
    }

    private static void CleanRepoExtras(string tempDirectory, string fileName)
    {
        var repoIndex = Path.Combine(tempDirectory, fileName);

        if (File.Exists(repoIndex))
        {
            File.Delete(repoIndex);
        }
    }

    private static void EnsureDirectoryIsClean(string tempDirectory)
    {
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, true);
        }

        Directory.CreateDirectory(tempDirectory);
    }
}