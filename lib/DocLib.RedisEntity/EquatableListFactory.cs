using IT.Collections.Factory;

namespace DocLib.RedisEntity;

public class EquatableListFactory : IEnumerableFactory
{
    public static readonly EquatableListFactory Default = new();

    public bool IsReadOnly => false;

    public IEnumerable<T> Empty<T>() => new EquatableList<T>();

    public IEnumerable<T> New<T>(int capacity) => new EquatableList<T>(capacity);

    public IEnumerable<T> New<T>(int capacity, EnumerableBuilder<T> builder)
    {
        if (capacity == 0) return new EquatableList<T>();
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        var list = new EquatableList<T>(capacity);

        builder(list.Add, false);

        return list;
    }

    public IEnumerable<T> New<T, TState>(int capacity, EnumerableBuilder<T, TState> builder, in TState state)
    {
        if (capacity == 0) return new EquatableList<T>();
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        var list = new EquatableList<T>(capacity);

        builder(list.Add, false, in state);

        return list;
    }
}