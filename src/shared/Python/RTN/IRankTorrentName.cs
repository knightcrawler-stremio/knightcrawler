namespace SharedContracts.Python.RTN;

public interface IRankTorrentName
{
    ParseTorrentTitleResponse Parse(string title);
    bool IsTrash(string title);
    bool TitleMatch(string title, string checkTitle);
}