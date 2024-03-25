namespace Metadata.Features.DeleteDownloadedImdbData;

public record DeleteDownloadedImdbDataRequest(string TitleBasicsFilePath, string TitleAkasFilePath, string EpisodesFilePath);
