namespace StackExchange.Redis.Entity.Factories;

public abstract class DictionaryFactoryBase : IDictionaryFactory
{
    public virtual IDictionary<TKey, TValue> Empty<TKey, TValue>() => New<TKey, TValue>(0);

    public IDictionary<TKey, TValue> New<TKey, TValue, TState>(int capacity, in TState state, EnumerableBuilder<KeyValuePair<TKey, TValue>, TState> builder)
    {
        if (capacity == 0) return Empty<TKey, TValue>();

        var dictionary = New<TKey, TValue>(capacity);

        builder(dictionary, in state);
        
        return dictionary;
    }

    protected abstract IDictionary<TKey, TValue> New<TKey, TValue>(int capacity);
}