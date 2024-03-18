namespace Producer.Features.PTN2;

public class ParseTorrentTitle
{
    private readonly Patterns _patterns = new();
    public string? TorrentName { get; set; }
    public object? Parts { get; set; }
    public object? PartSlices { get; set; }
    public object? MatchSlices { get; set; }
    public object? Standardise { get; set; }
    public object? CoherentTypes { get; set; }
    public string? PostTitlePattern { get; set; }

    public ParseTorrentTitle()
    {
        TorrentName = null;
        Parts = null;
        PartSlices = null;
        MatchSlices = null;
        Standardise = null;
        CoherentTypes = null;
        PostTitlePattern = $"(?:{Extras.LinkPatterns(_patterns.PtnPatterns["season"])}|{Extras.LinkPatterns(_patterns.PtnPatterns["year"])}|720p|1080p)";
    }

    private void Part(string name, MatchSlice matchSlice, object clean, bool overwrite = false)
    {
        if (overwrite || !Parts.ContainsKey(name))
        {
            if (CoherentTypes)
            {
                if (name != "title" && name != "episodeName" && !(clean is bool))
                {
                    if (!(clean is List<object>))
                    {
                        clean = new List<object> { clean };
                    }
                }
            }
            else
            {
                if (clean is List<object> cleanList && cleanList.Count == 1)
                {
                    clean = cleanList[0];  // Avoids making a list if it only has 1 element
                }
            }

            Parts[name] = clean;
            PartSlices[name] = matchSlice;
        }

        // Ignored patterns will still be considered 'matched' to remove them from excess.
        if (matchSlice != null)
        {
            MatchSlices.Add(matchSlice);
        }
    }
}