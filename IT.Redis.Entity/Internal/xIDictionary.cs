namespace IT.Redis.Entity.Internal;

internal static class xIDictionary
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key) where TValue : new()
    {
        if (!dic.TryGetValue(key, out var value))
        {
            value = new TValue();
            dic.Add(key, value);
        }

        return value;
    }
}