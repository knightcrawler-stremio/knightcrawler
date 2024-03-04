namespace Producer.Features.Text;

public class FuzzyStringSearcher(IEnumerable<string> records, SearchOptions<string>? options = null) : IFuzzySearcher<string>
{
    private readonly IReadOnlyCollection<string> _records = records.ToList();
    private readonly SearchOptions<string> _options = options ?? new SearchOptions<string>();

    public IReadOnlyCollection<ExtractedResult<string>> Search(string text) => 
        Process.ExtractSorted(text, _records, cutoff: _options.Threshold).ToList();
}
