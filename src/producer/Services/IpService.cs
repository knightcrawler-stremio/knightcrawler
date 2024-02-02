namespace Producer.Services;

public class IpService(ILogger<IpService> logger, IHttpClientFactory httpClientFactory) : IIpService
{
    public async Task GetPublicIpAddress()
    {
        var client = httpClientFactory.CreateClient("Scraper");
        client.DefaultRequestHeaders.UserAgent.ParseAdd("curl");
        var request = await client.GetStringAsync("http://ifconfig.me");

        logger.LogInformation("Public IP Address: {PublicIPAddress}", request);
    }
}