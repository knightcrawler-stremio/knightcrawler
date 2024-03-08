namespace Metadata.Features.Jobs;

public abstract class BaseJob : IMetadataJob
{
    public abstract bool IsScheduelable { get; }

    public abstract string JobName { get; }

    public abstract Task Invoke();
}
