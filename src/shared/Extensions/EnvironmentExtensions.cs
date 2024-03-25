namespace SharedContracts.Extensions;

public static class EnvironmentExtensions
{
    public static bool GetEnvironmentVariableAsBool(this string prefix, string varName, bool fallback = false)
    {
        var fullVarName = GetFullVariableName(prefix, varName);

        var str = Environment.GetEnvironmentVariable(fullVarName);

        if (string.IsNullOrEmpty(str))
        {
            return fallback;
        }

        return str.Trim().ToLower() switch
        {
            "true" => true,
            "yes" => true,
            "1" => true,
            _ => false,
        };
    }

    public static int GetEnvironmentVariableAsInt(this string prefix, string varName, int fallback = 0)
    {
        var fullVarName = GetFullVariableName(prefix, varName);

        var str = Environment.GetEnvironmentVariable(fullVarName);

        if (string.IsNullOrEmpty(str))
        {
            return fallback;
        }

        return int.TryParse(str, out var result) ? result : fallback;
    }

    public static string GetRequiredEnvironmentVariableAsString(this string prefix, string varName)
    {
        var fullVarName = GetFullVariableName(prefix, varName);

        var str = Environment.GetEnvironmentVariable(fullVarName);

        if (string.IsNullOrEmpty(str))
        {
            throw new InvalidOperationException($"Environment variable {fullVarName} is not set");
        }

        return str;
    }

    public static string GetOptionalEnvironmentVariableAsString(this string prefix, string varName, string? fallback = null)
    {
        var fullVarName = GetFullVariableName(prefix, varName);

        var str = Environment.GetEnvironmentVariable(fullVarName);

        if (string.IsNullOrEmpty(str))
        {
            return fallback;
        }

        return str;
    }

    private static string GetFullVariableName(string prefix, string varName) => $"{prefix}_{varName}";
}
