using System.Collections.Concurrent;

namespace StackExchange.Redis.Entity.Factories;

public class ConcurrentBagFactory : EnumerableFactory
{
    public static readonly ConcurrentBagFactory Default = new();

    protected override IEnumerable<T> New<T>(int capacity) => new ConcurrentBag<T>();
}