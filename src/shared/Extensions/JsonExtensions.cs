namespace SharedContracts.Extensions;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        NumberHandling = JsonNumberHandling.Strict,
    };
    
    public static string AsJson<T>(this T obj) => JsonSerializer.Serialize(obj, JsonSerializerOptions);
}