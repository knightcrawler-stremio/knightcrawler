namespace SharedContracts.Extensions;

public static class DictionaryExtensions
{
    public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TSource, TKey, TValue>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector) where TKey : notnull
    {
        var concurrentDictionary = new ConcurrentDictionary<TKey, TValue>();

        foreach (var element in source)
        {
            concurrentDictionary.TryAdd(keySelector(element), valueSelector(element));
        }

        return concurrentDictionary;
    }
}