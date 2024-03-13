namespace Producer.Features.PTN;

public interface IPtnHandler
{
    void RegisterHandlers(IParseTorrentName parser);
}