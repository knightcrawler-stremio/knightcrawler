namespace Metadata.Features.ClearExistingImdbData;

public record ClearExistingImdbDataRequest(string TitleBasicsFilePath, string TitleAkasFilePath, string EpisodesFilePath);
