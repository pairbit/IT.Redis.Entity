namespace IT.Redis.Entity;

public interface IKeyRebuilder
{
    byte[] RebuildKey<TKey1>(byte[]? key, byte bits, in TKey1 key1);

    byte[] RebuildKey<TKey1, TKey2>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2);

    byte[] RebuildKey<TKey1, TKey2, TKey3>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3);

    byte[] RebuildKey<TKey1, TKey2, TKey3, TKey4>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4);

    byte[] RebuildKey<TKey1, TKey2, TKey3, TKey4, TKey5>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5);

    byte[] RebuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6);

    byte[] RebuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7);

    byte[] RebuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7, TKey8>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7, in TKey8 key8);
}