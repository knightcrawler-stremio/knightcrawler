namespace Producer.Models.Configuration;

public class GithubConfiguration
{
    private const string Prefix = "GITHUB";
    private const string PatVariable = "PAT";
    
    public string? PAT { get; init; } =  Prefix.GetOptionalEnvironmentVariableAsString(PatVariable);
}