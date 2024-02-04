namespace Producer.Models.Configuration;

public class GithubConfiguration
{
    public const string SectionName = "GithubSettings";
    public const string Filename = "github.json";
    
    public string? PAT { get; set; }
}