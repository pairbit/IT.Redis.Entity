using StackExchange.Redis.Entity.Internal.Collections;

namespace StackExchange.Redis.Entity.Factories;

public class ReadOnlyLinkedListFactory : IEnumerableFactory
{
    public static readonly ReadOnlyLinkedListFactory Default = new();

    public IEnumerable<T> Empty<T>() => ReadOnlyCollection<T>.Empty;

    public IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return ReadOnlyCollection<T>.Empty;

        var list = new LinkedList<T>();

        builder(list, in state);

        return new ReadOnlyCollection<T>(list);
    }
}