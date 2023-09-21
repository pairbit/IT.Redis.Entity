#if NETCOREAPP3_1_OR_GREATER

using System.Collections.Immutable;

namespace StackExchange.Redis.Entity.Factories;

public class ImmutableDictionaryFactory : IDictionaryFactory
{
    public static readonly ImmutableDictionaryFactory Default = new();

    public IDictionary<TKey, TValue> Empty<TKey, TValue>() => ImmutableDictionary<TKey, TValue>.Empty;

    public IDictionary<TKey, TValue> New<TKey, TValue, TState>(int capacity, in TState state, EnumerableBuilder<KeyValuePair<TKey, TValue>, TState> builder)
    {
        if (capacity == 0) return ImmutableDictionary<TKey, TValue>.Empty;

        var dictionary = new Dictionary<TKey, TValue>(capacity);

        builder(dictionary, in state);

        return ImmutableDictionary<TKey, TValue>.Empty.AddRange(dictionary);
    }
}

#endif