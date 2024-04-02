namespace SharedContracts.Python.RTN;

public interface IRankTorrentName
{
    ParseTorrentTitleResponse Parse(string title, bool trashGarbage = true, bool logErrors = false, bool throwOnErrors = false);
    List<ParseTorrentTitleResponse?> BatchParse(IReadOnlyCollection<string> titles, int chunkSize = 500, int workers = 20, bool trashGarbage = true, bool logErrors = false, bool throwOnErrors = false);
}