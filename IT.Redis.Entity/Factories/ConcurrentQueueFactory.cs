using System.Collections.Concurrent;

namespace IT.Redis.Entity.Factories;

public class ConcurrentQueueFactory : EnumerableFactory
{
    public static readonly ConcurrentQueueFactory Default = new();

    protected override IEnumerable<T> New<T>(int capacity) => new ConcurrentQueue<T>();
}