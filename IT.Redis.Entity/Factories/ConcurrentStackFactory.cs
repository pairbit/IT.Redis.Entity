using System.Collections.Concurrent;

namespace IT.Redis.Entity.Factories;

public class ConcurrentStackFactory : EnumerableFactory
{
    public static readonly ConcurrentStackFactory Default = new();

    protected override IEnumerable<T> New<T>(int capacity) => new ConcurrentStack<T>();
}