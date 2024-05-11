namespace IT.Redis.Entity;

public interface IKeyBuilder
{
    byte[] BuildKey<TKey1>(in TKey1 key1);

    byte[] BuildKey<TKey1, TKey2>(in TKey1 key1, in TKey2 key2);

    byte[] BuildKey<TKey1, TKey2, TKey3>(in TKey1 key1, in TKey2 key2, in TKey3 key3);

    byte[] BuildKey<TKey1, TKey2, TKey3, TKey4>(in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4);

    byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5>(in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5);

    byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6>(in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6);

    byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7>(in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7);

    byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7, TKey8>(in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7, in TKey8 key8);
}