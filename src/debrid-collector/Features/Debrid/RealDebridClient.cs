namespace DebridCollector.Features.Debrid;

public class RealDebridClient(HttpClient client) : IDebridHttpClient
{
    private const string TorrentsInstantAvailability = "torrents/instantAvailability/";

    public async Task<IReadOnlyList<TorrentMetadataResponse>> GetMetadataAsync(IReadOnlyCollection<PerformMetadataRequest> requests, CancellationToken cancellationToken = default)
    {
        var responseAsString = await client.GetStringAsync($"{TorrentsInstantAvailability}{string.Join("/", requests.Select(x => x.InfoHash.ToLowerInvariant()))}", cancellationToken);

        var document = JsonDocument.Parse(responseAsString);
        
        var torrentMetadataResponses = new List<TorrentMetadataResponse>();
        
        foreach (var request in requests)
        {
            if (document.RootElement.TryGetProperty(request.InfoHash.ToLowerInvariant(), out var dataElement) && 
                dataElement.ValueKind == JsonValueKind.Object &&
                dataElement.TryGetProperty("rd", out var rdDataElement) && 
                rdDataElement.ValueKind == JsonValueKind.Array &&
                rdDataElement.GetArrayLength() > 0)
            {
                MapResponseToMetadata(rdDataElement, torrentMetadataResponses, request);
                continue;
            }

            torrentMetadataResponses.Add(new(request.CorrelationId, new()));
        }
        
        return torrentMetadataResponses;
    }

    private static void MapResponseToMetadata(JsonElement rdDataElement, List<TorrentMetadataResponse> torrentMetadataResponses, PerformMetadataRequest request)
    {
        var metaData = new FileDataDictionary();

        foreach (var item in rdDataElement.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in item.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.Object)
                    {
                        var fileData = new FileData();
                    
                        if (property.Value.TryGetProperty("filename", out var filenameElement) && filenameElement.ValueKind == JsonValueKind.String)
                        {
                            fileData.Filename = filenameElement.GetString();
                        }
                    
                        if (property.Value.TryGetProperty("filesize", out var filesizeElement) && filesizeElement.ValueKind == JsonValueKind.Number)
                        {
                            fileData.Filesize = filesizeElement.GetInt64();
                        }
                    
                        metaData[property.Name] = fileData;
                    }
                }
            }
        }

        torrentMetadataResponses.Add(new(request.CorrelationId, metaData));
    }
}