namespace Producer.Features.PTN;

public static class DictionaryExtensions
{
    public static List<T> GetValueAsList<T>(this Dictionary<string, object> dictionary, string key)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            if (value is T item)
            {
                return [item];
            }

            if (value is List<T> list)
            {
                return list;
            }

            if (value is string str && typeof(T) == typeof(string))
            {
                return str.Split(',').Cast<T>().ToList();
            }
        }

        return [];
    }
    
    public static bool GetValueAsBool(this Dictionary<string, object> dictionary, string key)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            if (value is string str && bool.TryParse(str, out var boolValue))
            {
                return boolValue;
            }
        }

        return false;
    }
    
    public static string GetValueAsString(this Dictionary<string, object> dictionary, string key)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value as string;
        }

        return null;
    }
}