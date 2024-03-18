namespace Producer.Features.PTN2;

public static class Transformers
{
    public static Func<string, string> None = input => input;
    public static Func<string?, string> Replace(string? value) => input => value?.Replace("$1", input, StringComparison.OrdinalIgnoreCase);
    public static Func<string?, string> Lowercase = input => input?.ToLower();
    public static Func<string?, string> Uppercase = input => input?.ToUpper();
    public static Func<string?, string> Integer = input => int.TryParse(input, out var result) ? result.ToString() : null;
    public static Func<string?, string> Boolean = input => !string.IsNullOrEmpty(input) ? bool.TrueString : null;
    public static Func<string?, string> Clean => input => input.Replace(" ", "", StringComparison.OrdinalIgnoreCase).Replace(".", "", StringComparison.OrdinalIgnoreCase).Replace("-", "", StringComparison.OrdinalIgnoreCase).ToLower();
}
