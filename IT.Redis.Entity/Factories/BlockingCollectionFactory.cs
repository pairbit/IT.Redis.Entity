using System.Collections.Concurrent;

namespace IT.Redis.Entity.Factories;

public class BlockingCollectionFactory : IEnumerableFactory
{
    public static readonly BlockingCollectionFactory Default = new();

    public IEnumerable<T> Empty<T>() => new BlockingCollection<T>();

    public IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return new BlockingCollection<T>();

        var queue = new ConcurrentQueue<T>();

        builder(queue, in state);

        return new BlockingCollection<T>(queue, capacity);
    }
}