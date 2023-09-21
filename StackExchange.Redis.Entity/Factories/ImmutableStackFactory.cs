#if NETCOREAPP3_1_OR_GREATER

using System.Collections.Immutable;

namespace StackExchange.Redis.Entity.Factories;

public class ImmutableStackFactory : IEnumerableFactory
{
    public static readonly ImmutableStackFactory Default = new();

    public IEnumerable<T> Empty<T>() => ImmutableStack<T>.Empty;

    public IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return ImmutableStack<T>.Empty;

        var array = new T[capacity];

        builder(array, in state);

        return ImmutableStack.Create(array);
    }
}

#endif