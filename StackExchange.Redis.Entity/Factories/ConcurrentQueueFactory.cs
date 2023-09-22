using System.Collections.Concurrent;

namespace StackExchange.Redis.Entity.Factories;

public class ConcurrentQueueFactory : EnumerableFactory
{
    public static readonly ConcurrentQueueFactory Default = new();

    protected override IEnumerable<T> New<T>(int capacity) => new ConcurrentQueue<T>();
}