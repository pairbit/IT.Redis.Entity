using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace StackExchange.Redis.Entity;

public class RedisEntityFields : IRedisEntityFields
{
    private readonly IReadOnlyDictionary<string, RedisValue> _dictionary;

    public RedisValue[] All { get; }

    public IEnumerable<string> Keys => _dictionary.Keys;

    public IEnumerable<RedisValue> Values => _dictionary.Values;

    public int Count => _dictionary.Count;

    public RedisValue this[string key] => _dictionary[key];

    public RedisEntityFields(IReadOnlyDictionary<string, RedisValue> dictionary)
    {
        _dictionary = dictionary;
        All = dictionary.Values.ToArray();
    }

    public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

    public bool TryGetValue(
        string key,
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        [MaybeNullWhen(false)]
#endif
        out RedisValue value) => _dictionary.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<string, RedisValue>> GetEnumerator() => _dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();
}