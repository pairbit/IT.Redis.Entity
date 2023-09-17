using System.Collections.ObjectModel;

namespace StackExchange.Redis.Entity.Factories;

public class ReadOnlyListFactory : IEnumerableFactory
{
    public static readonly ReadOnlyListFactory Default = new();

    public IEnumerable<T> Empty<T>() => Cache<T>.Empty;

    public IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return Cache<T>.Empty;

        var list = new List<T>(capacity);

        builder(list, in state);

        return new ReadOnlyCollection<T>(list);
    }

    static class Cache<T>
    {
        public readonly static ReadOnlyCollection<T> Empty = new(new List<T>());
    }
}