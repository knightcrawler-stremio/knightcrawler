namespace Producer.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value) =>
        string.IsNullOrEmpty(value);
}
