namespace Scraper.Extensions;

public static class ConfigurationExtensions
{
    private const string ConfigurationFolder = "Configuration";
    private const string LoggingConfig = "logging.json";
    
    public static IConfigurationBuilder AddScrapeConfiguration(this IConfigurationBuilder configuration)
    {
        configuration.SetBasePath(Path.Combine(AppContext.BaseDirectory, ConfigurationFolder));
        
        configuration.AddJsonFile(LoggingConfig, false, true);
        configuration.AddJsonFile(ScrapeConfiguration.Filename, false, true);
        configuration.AddJsonFile(RabbitMqConfiguration.Filename, false, true);
        configuration.AddJsonFile(GithubConfiguration.Filename, false, true);
        
        configuration.AddEnvironmentVariables();

        configuration.AddUserSecrets<Program>();
        
        return configuration;
    }
}