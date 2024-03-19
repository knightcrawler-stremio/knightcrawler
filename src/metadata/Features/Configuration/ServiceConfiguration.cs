namespace Metadata.Features.Configuration;

public class ServiceConfiguration
{
    private const string Prefix = "METADATA";
    private const string InsertBatchSizeVariable = "INSERT_BATCH_SIZE";

    public int InsertBatchSize { get; init; } = Prefix.GetEnvironmentVariableAsInt(InsertBatchSizeVariable, 25_000);
}
