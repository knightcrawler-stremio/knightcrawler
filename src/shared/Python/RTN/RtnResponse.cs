namespace SharedContracts.Python.RTN;

public class RtnResponse
{
    [JsonPropertyName("raw_title")]
    public string? RawTitle { get; set; }
    
    [JsonPropertyName("parsed_title")]
    public string? ParsedTitle { get; set; }
    
    [JsonPropertyName("fetch")]
    public bool Fetch { get; set; }
    
    [JsonPropertyName("is_4k")]
    public bool Is4K { get; set; }
    
    [JsonPropertyName("is_multi_audio")]
    public bool IsMultiAudio { get; set; }
    
    [JsonPropertyName("is_multi_subtitle")]
    public bool IsMultiSubtitle { get; set; }
    
    [JsonPropertyName("is_complete")]
    public bool IsComplete { get; set; }
    
    [JsonPropertyName("year")]
    public int Year { get; set; }
    
    [JsonPropertyName("resolution")]
    public List<string>? Resolution { get; set; }
    
    [JsonPropertyName("quality")]
    public List<string>? Quality { get; set; }
    
    [JsonPropertyName("season")]
    public List<int>? Season { get; set; }
    
    [JsonPropertyName("episode")]
    public List<int>? Episode { get; set; }
    
    [JsonPropertyName("codec")]
    public List<string>? Codec { get; set; }
    
    [JsonPropertyName("audio")]
    public List<string>? Audio { get; set; }
    
    [JsonPropertyName("subtitles")]
    public List<string>? Subtitles { get; set; }
    
    [JsonPropertyName("language")]
    public List<string>? Language { get; set; }
    
    [JsonPropertyName("bit_depth")]
    public List<int>? BitDepth { get; set; }
    
    [JsonPropertyName("hdr")]
    public string? Hdr { get; set; }
    
    [JsonPropertyName("proper")]
    public bool Proper { get; set; }
    
    [JsonPropertyName("repack")]
    public bool Repack { get; set; }
    
    [JsonPropertyName("remux")]
    public bool Remux { get; set; }
    
    [JsonPropertyName("upscaled")]
    public bool Upscaled { get; set; }
    
    [JsonPropertyName("remastered")]
    public bool Remastered { get; set; }
    
    [JsonPropertyName("directors_cut")]
    public bool DirectorsCut { get; set; }
    
    [JsonPropertyName("extended")]
    public bool Extended { get; set; }
    
    public bool IsMovie => (Season == null && Episode == null) || (Season?.Count == 0 && Episode?.Count == 0);
    
    public string ToJson() => this.AsJson();
}