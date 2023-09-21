#if NETCOREAPP3_1_OR_GREATER

using System.Collections.Immutable;

namespace StackExchange.Redis.Entity.Factories;

public class ImmutableHashSetFactory : IEnumerableFactory
{
    public static readonly ImmutableHashSetFactory Default = new();

    public IEnumerable<T> Empty<T>() => ImmutableHashSet<T>.Empty;

    public IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return ImmutableHashSet<T>.Empty;

        var array = new T[capacity];

        builder(array, in state);

        return ImmutableHashSet<T>.Empty.Union(array);
    }
}

#endif