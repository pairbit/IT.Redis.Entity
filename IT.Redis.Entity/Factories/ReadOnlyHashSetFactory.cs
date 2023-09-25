#if NET6_0_OR_GREATER

using IT.Redis.Entity.Internal.Collections;

namespace IT.Redis.Entity.Factories;

public class ReadOnlyHashSetFactory : IEnumerableFactory
{
    public static readonly ReadOnlyHashSetFactory Default = new();

    public IEnumerable<T> Empty<T>() => ReadOnlySet<T>.Empty;

    public IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return ReadOnlySet<T>.Empty;

        var hashSet = new HashSet<T>(capacity, null);

        builder(hashSet, in state);

        return new ReadOnlySet<T>(hashSet);
    }
}

#endif