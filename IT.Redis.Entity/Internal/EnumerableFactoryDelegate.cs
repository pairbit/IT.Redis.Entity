namespace IT.Redis.Entity.Internal;

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

internal class DictionaryFactoryDelegate<TDictionary, TKey, TValue> : IDictionaryFactory<TDictionary, TKey, TValue>
    where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
{
    private readonly DictionaryFactory<TDictionary, TKey, TValue> _factory;

    public DictionaryFactoryDelegate(DictionaryFactory<TDictionary, TKey, TValue> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public TDictionary Empty() => _factory(0);

    public TDictionary New<TState>(int capacity, in TState state, EnumerableBuilder<KeyValuePair<TKey, TValue>, TState> builder)
    {
        if (capacity == 0) return _factory(0);

        var dictionary = _factory(capacity);

        builder(dictionary, in state);

        return dictionary;
    }
}