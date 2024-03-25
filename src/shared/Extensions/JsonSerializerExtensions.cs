namespace SharedContracts.Extensions;

public static class JsonSerializerExtensions
{
    public static string ToJson<T>(this T value) => JsonSerializer.Serialize(value);
}
