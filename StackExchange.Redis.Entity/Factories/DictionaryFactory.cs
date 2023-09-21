namespace StackExchange.Redis.Entity.Factories;

public class DictionaryFactory : IDictionaryFactory
{
    public static readonly DictionaryFactory Default = new();

    public virtual IDictionary<TKey, TValue> Empty<TKey, TValue>() => new Dictionary<TKey, TValue>();

    public IDictionary<TKey, TValue> New<TKey, TValue, TState>(int capacity, in TState state, EnumerableBuilder<KeyValuePair<TKey, TValue>, TState> builder)
    {
        if (capacity == 0) return Empty<TKey, TValue>();

        var dictionary = New<TKey, TValue>(capacity);

        builder(dictionary, in state);
        
        return dictionary;
    }

    protected virtual IDictionary<TKey, TValue> New<TKey, TValue>(int capacity) => new Dictionary<TKey, TValue>(capacity, null);
}