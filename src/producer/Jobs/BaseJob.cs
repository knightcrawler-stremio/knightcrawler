namespace Scraper.Jobs;

public abstract class BaseJob(ICrawlerProvider crawlerProvider) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        if (context.RefireCount > 5)
        {
            throw new InvalidOperationException("Job failed too many times");
        }

        try
        {
            await crawlerProvider.Get(Crawler).Execute();
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);
        }
    }

    protected abstract string Crawler { get; }
}