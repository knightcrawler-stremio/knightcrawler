namespace SharedContracts.Models;

public class SubtitleFile
{
    public int Id { get; set; }
    public string? InfoHash { get; set; }
    public int FileIndex { get; set; }
    public int FileId { get; set; }
    public string? Title { get; set; }
}