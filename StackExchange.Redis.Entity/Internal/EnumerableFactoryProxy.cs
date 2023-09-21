namespace StackExchange.Redis.Entity.Internal;

internal class EnumerableFactoryProxy<TEnumerable, T> : IEnumerableFactory<TEnumerable, T> where TEnumerable : IEnumerable<T>
{
    private readonly IEnumerableFactory _factory;

    public EnumerableFactoryProxy(IEnumerableFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public TEnumerable Empty() => (TEnumerable)_factory.Empty<T>();

    public TEnumerable New<TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder)
        => (TEnumerable)_factory.New(capacity, in state, builder);
}

internal class DictionaryFactoryProxy<TDictionary, TKey, TValue> : IDictionaryFactory<TDictionary, TKey, TValue>
    where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
{
    private readonly IDictionaryFactory _factory;

    public DictionaryFactoryProxy(IDictionaryFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public TDictionary Empty() => (TDictionary)_factory.Empty<TKey, TValue>();

    public TDictionary New<TState>(int capacity, in TState state, EnumerableBuilder<KeyValuePair<TKey, TValue>, TState> builder)
        => (TDictionary)_factory.New(capacity, in state, builder);
}