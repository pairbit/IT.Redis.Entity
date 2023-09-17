namespace StackExchange.Redis.Entity.Internal;

internal class EnumerableFactoryDelegate<TEnumerable, T> : IEnumerableFactory<TEnumerable, T> where TEnumerable : IEnumerable<T>
{
    private readonly EnumerableFactory<TEnumerable, T> _factory;

    public EnumerableFactoryDelegate(EnumerableFactory<TEnumerable, T> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public TEnumerable Empty() => _factory(0);

    public TEnumerable New<TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
    {
        if (capacity == 0) return _factory(0);

        var enumerable = _factory(capacity);

        builder(enumerable, in state);

        return enumerable;
    }
}