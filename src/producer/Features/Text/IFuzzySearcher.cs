namespace Producer.Features.Text;

public interface IFuzzySearcher<T>
{
    IReadOnlyCollection<ExtractedResult<T>> Search(string text);
}