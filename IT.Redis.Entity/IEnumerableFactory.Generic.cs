namespace IT.Redis.Entity;

public interface IEnumerableFactory<TEnumerable, T> where TEnumerable : IEnumerable<T>
{
    TEnumerable Empty();

    TEnumerable New<TState>(int capacity, in TState state, EnumerableBuilder<T, TState> builder);
}

public delegate TDictionary DictionaryFactory<TDictionary, TKey, TValue>(int capacity) where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>;

public interface IDictionaryFactory<TDictionary, TKey, TValue> : IEnumerableFactory<TDictionary, KeyValuePair<TKey, TValue>>
    where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
{

}