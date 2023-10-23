using IT.Collections.Factory;
using IT.Collections.Factory.Generic;

namespace IT.Redis.Entity.Formatters;

public class UnmanagedDictionaryFormatter<TDictionary, TKey, TValue> : UnmanagedEnumerableFormatter<TDictionary, KeyValuePair<TKey, TValue>>
    where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : unmanaged
    where TValue : unmanaged
{
    public UnmanagedDictionaryFormatter(IDictionaryFactory<TDictionary, TKey, TValue> factory) : base(factory) { }

    public UnmanagedDictionaryFormatter(DictionaryFactory<TDictionary, TKey, TValue> factory,
        Action<TDictionary, KeyValuePair<TKey, TValue>> add, EnumerableKind kind = EnumerableKind.None)
        : base(new DictionaryFactoryDelegate<TDictionary, TKey, TValue>(
            factory, (items, item) => { add(items, item); return true; }, kind))
    { }
}