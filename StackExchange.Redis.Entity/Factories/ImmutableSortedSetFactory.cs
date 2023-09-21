#if NETCOREAPP3_1_OR_GREATER

using System.Collections.Immutable;

namespace StackExchange.Redis.Entity.Factories;

public class ImmutableSortedSetFactory : IEnumerableFactory
{
    public static readonly ImmutableSortedSetFactory Default = new();

    public IEnumerable<T> Empty<T>() => ImmutableSortedSet<T>.Empty;

    public IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return ImmutableSortedSet<T>.Empty;

        var array = new T[capacity];

        builder(array, in state);

        return ImmutableSortedSet<T>.Empty.Union(array);
    }
}

#endif