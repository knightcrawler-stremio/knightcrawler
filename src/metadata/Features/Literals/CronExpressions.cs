namespace Metadata.Features.Literals;

public static class CronExpressions
{
    public const string EveryHour = "0 0 * * *";
    public const string EveryDay = "0 0 0 * *";
    public const string EveryWeek = "0 0 * * 0";
    public const string EveryMonth = "0 0 0 * *";
}
