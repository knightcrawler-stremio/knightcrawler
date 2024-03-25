namespace SharedContracts.Dapper;

public class DapperResult<TSuccess, TFailure>
{
    public TSuccess Success { get; }
    public TFailure Failure { get; }
    public bool IsSuccess { get; }

    private DapperResult(TSuccess success, TFailure failure, bool isSuccess)
    {
        Success = success;
        Failure = failure;
        IsSuccess = isSuccess;
    }

    public static DapperResult<TSuccess, TFailure> Ok(TSuccess success) => 
        new(success, default, true);

    public static DapperResult<TSuccess, TFailure> Fail(TFailure failure) => 
        new(default, failure, false);
}