namespace Producer.Features.PTN;

public class HandlerResult
{
    public string RawMatch { get; set; } = default!;
    public int MatchIndex { get; set; }
    public bool? Remove { get; set; }
    public bool? SkipFromTitle { get; set; }
}
