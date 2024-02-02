namespace Producer.Jobs;

[DisallowConcurrentExecution]
public class IPJob(IIpService ipService) : IJob
{
    private const string JobName = nameof(IPJob);
    public static readonly JobKey Key = new(JobName, nameof(Jobs));
    public static readonly TriggerKey Trigger = new($"{JobName}-trigger", nameof(Jobs));
    
    public Task Execute(IJobExecutionContext context)
    {
        return ipService.GetPublicIpAddress();
    }
}