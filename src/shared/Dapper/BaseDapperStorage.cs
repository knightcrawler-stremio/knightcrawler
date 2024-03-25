namespace SharedContracts.Dapper;

public abstract class BaseDapperStorage(ILogger<IDataStorage> logger, PostgresConfiguration configuration)
{
    protected async Task ExecuteCommandAsync(Func<NpgsqlConnection, Task> operation, string errorMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(configuration.StorageConnectionString);
            await connection.OpenAsync(cancellationToken);

            await operation(connection);
        }
        catch (Exception e)
        {
            logger.LogError(e, errorMessage);
        }
    }
    
    protected async Task<TResult> ExecuteCommandAsync<TResult>(Func<NpgsqlConnection, Task<TResult>> operation, string errorMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(configuration.StorageConnectionString);
            await connection.OpenAsync(cancellationToken);

            var result = await operation(connection);
            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, errorMessage);
            throw;
        }
    }
    
    protected async Task<DapperResult<TResult, TFailure>> ExecuteCommandAsync<TResult, TFailure>(Func<NpgsqlConnection, Task<TResult>> operation, Func<Exception, TFailure> createFailureResult, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(configuration.StorageConnectionString);
            await connection.OpenAsync(cancellationToken);

            var result = await operation(connection);
            return DapperResult<TResult, TFailure>.Ok(result);
        }
        catch (Exception e)
        {
            var failureResult = createFailureResult(e);
            return DapperResult<TResult, TFailure>.Fail(failureResult);
        }
    }
}