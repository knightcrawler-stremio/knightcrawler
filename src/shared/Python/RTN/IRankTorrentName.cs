namespace SharedContracts.Python.RTN;

public interface IRankTorrentName
{
    ParseTorrentTitleResponse Parse(string title);
}