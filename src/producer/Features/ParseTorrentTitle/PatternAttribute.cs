namespace Producer.Features.ParseTorrentTitle;

[AttributeUsage(AttributeTargets.Property)]
public class PatternAttribute : Attribute
{
    public string Regex { get; set; } = default!;
    public string AlternateRegex { get; set; } = default!;
    public RegexOptions Options { get; set; }
    public string Replacements { get; set; } = default!;
}
