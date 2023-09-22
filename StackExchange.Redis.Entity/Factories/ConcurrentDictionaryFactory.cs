using System.Collections.Concurrent;

namespace StackExchange.Redis.Entity.Factories;

public class ConcurrentDictionaryFactory : DictionaryFactoryBase
{
    public static readonly ConcurrentDictionaryFactory Default = new();

    protected override IDictionary<TKey, TValue> New<TKey, TValue>(int capacity)
        => new ConcurrentDictionary<TKey, TValue>(Environment.ProcessorCount, capacity);
}