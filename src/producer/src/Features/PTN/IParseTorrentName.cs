namespace Producer.Features.PTN;

public interface IParseTorrentName
{
    void AddHandler(string? handlerName, object? handler, Func<string, string, string>? transformer, ParseTorrentNameOptions options);
    ParsedTorrentResult Parse(string title);
    TorrentType GetTorrentTypeByName(string name);
}
