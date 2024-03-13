namespace Producer.Features.PTN;

public interface IPTN
{
    void AddHandler(string? handlerName, object? handler, Func<string, string, string>? transformer, Options options);
    Dictionary<string, object> Parse(string title);
}
