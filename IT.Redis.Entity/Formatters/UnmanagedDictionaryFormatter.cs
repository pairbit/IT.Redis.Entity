using IT.Redis.Entity.Internal;

namespace IT.Redis.Entity.Formatters;

public class UnmanagedDictionaryFormatter<TDictionary, TKey, TValue> : UnmanagedEnumerableFormatter<TDictionary, KeyValuePair<TKey, TValue>>
    where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : unmanaged
    where TValue : unmanaged
{
    public UnmanagedDictionaryFormatter(IDictionaryFactory<TDictionary, TKey, TValue> factory) : base(factory) { }

    public UnmanagedDictionaryFormatter(DictionaryFactory<TDictionary, TKey, TValue> factory)
        : base(new DictionaryFactoryDelegate<TDictionary, TKey, TValue>(factory ?? throw new ArgumentNullException(nameof(factory))))
    { }
}