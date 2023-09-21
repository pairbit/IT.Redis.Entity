#if NETCOREAPP3_1_OR_GREATER

using System.Collections.Immutable;

namespace StackExchange.Redis.Entity.Factories;

public class ImmutableQueueFactory : IEnumerableFactory
{
    public static readonly ImmutableQueueFactory Default = new();

    public IEnumerable<T> Empty<T>() => ImmutableQueue<T>.Empty;

    public IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return ImmutableQueue<T>.Empty;

        var array = new T[capacity];

        builder(array, in state);
        
        return ImmutableQueue.Create(array);
    }
}

#endif