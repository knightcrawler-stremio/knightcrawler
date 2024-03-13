namespace Producer.Features.PTN;

public static class Transformers
{
    public static Func<string, string, string> None = (_, input) => input;

    public static Func<string, string?, string> Value(string? value) => (_, input) => value?.Replace("$1", input, StringComparison.OrdinalIgnoreCase);

    public static Func<string, string?, string> Lowercase = (_, input) => input?.ToLower();

    public static Func<string, string?, string> Uppercase = (_, input) => input?.ToUpper();

    public static Func<string, string?, string> Integer = (_, input) => int.TryParse(input, out var result) ? result.ToString() : null;

    public static Func<string, string?, string> Boolean = (_, input) => bool.TryParse(input, out var result) ? result.ToString() : null;

    public static Func<string, string?, string> Date(string dateFormat) => (_, input) =>
    {
        var sanitized = ParserRegex.Word().Replace(input, " ").Trim();
        var date = DateTime.ParseExact(sanitized, dateFormat, CultureInfo.InvariantCulture);

        return date.ToString("yyyy-MM-dd");
    };

    public static Func<string, string?, string> Range = (match, input) =>
    {
        var array = ParserRegex.Numbers().Replace(input, " ")
            .Trim()
            .Split(" ")
            .Select(int.Parse)
            .ToArray();

        if (array.Length == 2 && array[0] < array[1])
        {
            array = Enumerable.Range(array[0], array[1] - array[0] + 1).ToArray();
        }
        return array.Skip(1)
            .Select(
                (number, idx) => new
                {
                    number, idx,
                })
            .Any(item => item.number - 1 != array[item.idx]) ? null : string.Join(",", array);
    };

    public static Func<string, string, string> YearRange = (match, input) =>
    {
        var parts = ParserRegex.YearDigits().Split(input).Select(int.Parse).ToArray();
        var start = parts[0];
        var end = parts[1];

        if (end < 100)
        {
            end = end + start - start % 100;
        }
        return end <= start ? null : $"{start}-{end}";
    };

    public static Func<string, string, string> Array(Func<string, string, string>? chain) => (match, input) => chain != null ? chain(match, input) : input;

    public static Func<string, string, string> UniqConcat(Func<string, string, string> chain) => (match, input) =>
    {
        var result = new List<string>();
        var value = chain(match, input);

        return result.Contains(value) ? string.Join(",", result) : string.Join(",", result.Concat(new[] { value }));
    };
}
