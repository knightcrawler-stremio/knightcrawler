namespace SharedContracts.Dapper;

public record InsertTorrentResult(bool Success, int InsertedCount = 0, string? ErrorMessage = null);
public record UpdatedTorrentResult(bool Success, int UpdatedCount = 0, string? ErrorMessage = null);
public record PageIngestedResult(bool Success, string? ErrorMessage = null);
