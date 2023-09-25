#if NETCOREAPP3_1_OR_GREATER

using System.Collections.Immutable;

namespace IT.Redis.Entity.Factories;

public class ImmutableArrayFactory : IEnumerableFactory
{
    public static readonly ImmutableArrayFactory Default = new();

    public IEnumerable<T> Empty<T>() => ImmutableArray<T>.Empty;

    public IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return ImmutableArray<T>.Empty;

        var array = new T[capacity];

        builder(array, in state);

        return ImmutableArray.Create(array);
    }
}

#endif