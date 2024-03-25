namespace DebridCollector.Features.Debrid;

public class RealDebridResponse : Dictionary<string, RdData?>
{
}

public class RdData
{
    [JsonPropertyName("rd")]
    public List<FileDataDictionary>? Rd { get; set; }
}

public class FileDataDictionary : Dictionary<string, FileData>
{
}

public class FileData
{
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }
    
    [JsonPropertyName("filesize")]
    public long? Filesize { get; set; }
}