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

#if NETSTANDARD2_0
    public bool TryGetValue(string key, out RedisValue value) => _dictionary.TryGetValue(key, out value);
#else
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out RedisValue value) => _dictionary.TryGetValue(key, out value);
#endif

    public IEnumerator<KeyValuePair<string, RedisValue>> GetEnumerator() => _dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();
}