using System.Collections.ObjectModel;

namespace StackExchange.Redis.Entity.Factories;

public class CollectionFactory : IEnumerableFactory
{
    public static readonly CollectionFactory Default = new();

    public IEnumerable<T> Empty<T>() => new Collection<T>();

    public IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return new Collection<T>();

        var list = new List<T>(capacity);

        builder(list, in state);

        return new Collection<T>(list);
    }
}