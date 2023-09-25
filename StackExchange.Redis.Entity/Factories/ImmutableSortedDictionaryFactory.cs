#if NETCOREAPP3_1_OR_GREATER

using System.Collections.Immutable;

namespace StackExchange.Redis.Entity.Factories;

public class ImmutableSortedDictionaryFactory : IDictionaryFactory
{
    public static readonly ImmutableSortedDictionaryFactory Default = new();

    public IDictionary<TKey, TValue> Empty<TKey, TValue>() where TKey : notnull => ImmutableSortedDictionary<TKey, TValue>.Empty;

    public IDictionary<TKey, TValue> New<TKey, TValue, TState>(int capacity, in TState state, EnumerableBuilder<KeyValuePair<TKey, TValue>, TState> builder) where TKey : notnull
    {
        if (capacity == 0) return ImmutableSortedDictionary<TKey, TValue>.Empty;

        var dictionary = new Dictionary<TKey, TValue>(capacity);

        builder(dictionary, in state);

        return ImmutableSortedDictionary<TKey, TValue>.Empty.AddRange(dictionary);
    }
}

#endif