namespace Metadata.Features.Jobs;

public interface IMetadataJob : IInvocable
{
    bool IsScheduelable { get; }
    string JobName { get; }
}
