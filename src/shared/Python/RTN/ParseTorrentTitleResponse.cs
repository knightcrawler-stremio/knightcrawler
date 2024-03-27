namespace SharedContracts.Python.RTN;

public record ParseTorrentTitleResponse(bool Success, string ParsedTitle, int? Year = null, int[]? Season = null, int[]? Episode = null)
{
    public bool IsMovie => Season == null && Episode == null;
}