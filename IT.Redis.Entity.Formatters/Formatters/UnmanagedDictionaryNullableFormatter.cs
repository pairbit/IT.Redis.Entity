﻿using IT.Collections.Factory.Generic;

namespace IT.Redis.Entity.Formatters;

public class UnmanagedDictionaryNullableFormatter<TDictionary, TKey, TValue> : UnmanagedEnumerableFormatter<TDictionary, KeyValuePair<TKey, TValue?>>
    where TDictionary : IEnumerable<KeyValuePair<TKey, TValue?>>
    where TKey : unmanaged
    where TValue : unmanaged
{
    public UnmanagedDictionaryNullableFormatter(IDictionaryFactory<TDictionary, TKey, TValue?> factory) : base(factory) { }

    public UnmanagedDictionaryNullableFormatter(DictionaryFactory<TDictionary, TKey, TValue?> factory,
        Action<TDictionary, KeyValuePair<TKey, TValue?>> add, bool reverse)
        : base(new DictionaryFactoryDelegate<TDictionary, TKey, TValue?>(factory, add, reverse))
    { }
}