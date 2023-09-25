namespace StackExchange.Redis.Entity;

public delegate void EnumerableBuilder<T, TState>(IEnumerable<T> buffer, in TState state);

public interface IEnumerableFactory
{
    IEnumerable<T> Empty<T>();

    IEnumerable<T> New<T, TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder);
}

public interface IDictionaryFactory
{
    IDictionary<TKey, TValue> Empty<TKey, TValue>() where TKey : notnull;

    IDictionary<TKey, TValue> New<TKey, TValue, TState>(int capacity, in TState state, EnumerableBuilder<KeyValuePair<TKey, TValue>, TState> builder) where TKey : notnull;
}