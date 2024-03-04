namespace Producer.Features.Text;

public class AdultContentConfiguration
{
    public const string SectionName = "AdultContentSettings";
    public const string Filename = "adultcontent.json";
    
    public bool Allow { get; set; }

    public List<string> Keywords { get; set; } = [];
    
    public int Threshold { get; set; }
}