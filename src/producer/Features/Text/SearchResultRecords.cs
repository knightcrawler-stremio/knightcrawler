namespace Producer.Features.Text;

public class SearchResultRecords
{
    public record struct ScoreInfo(int Errors, int CurrentLocation, int ExpectedLocation, int Distance,
        bool IgnoreLocation);

    public record struct SearchResult(bool IsMatch, double Score);

    public record struct Index(List<Chunk> Chunks, string Pattern);

    public record struct Chunk(int StartIndex, string Pattern, Dictionary<char, int> Alphabet);
    
    public record struct SearchResult<T>(T Value, double Score);
}

