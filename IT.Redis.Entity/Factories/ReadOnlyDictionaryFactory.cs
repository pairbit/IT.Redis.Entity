using System.Collections.ObjectModel;

namespace IT.Redis.Entity.Factories;

public class ReadOnlyDictionaryFactory : IDictionaryFactory
{
    public static readonly ReadOnlyDictionaryFactory Default = new();

    public IDictionary<TKey, TValue> Empty<TKey, TValue>() where TKey : notnull => Cache<TKey, TValue>.Empty;

    public IDictionary<TKey, TValue> New<TKey, TValue, TState>(int capacity, in TState state, EnumerableBuilder<KeyValuePair<TKey, TValue>, TState> builder) where TKey : notnull
    {
        if (capacity == 0) return Cache<TKey, TValue>.Empty;

        var dictionary = new Dictionary<TKey, TValue>(capacity);

        builder(dictionary, in state);

        return new ReadOnlyDictionary<TKey, TValue>(dictionary);
    }

    static class Cache<TKey, TValue> where TKey : notnull
    {
        public readonly static ReadOnlyDictionary<TKey, TValue> Empty = new(new Dictionary<TKey, TValue>());
    }
}