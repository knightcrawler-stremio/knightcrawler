namespace QBitCollector.Features.Trackers;

public interface ITrackersService
{
    Task<List<string>> GetTrackers();
}