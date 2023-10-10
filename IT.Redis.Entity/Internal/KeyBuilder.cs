namespace IT.Redis.Entity.Internal;

internal class KeyBuilder
{
    private readonly List<object> _serializers = new(5);
    private readonly byte[] _prefix;
    private readonly byte _separator = (byte)':';

    public KeyBuilder(byte[]? prefix)
    {
        _prefix = prefix ?? Array.Empty<byte>();
    }

    public void AddSerializer(object serializer) => _serializers.Add(serializer);

    public byte[] Build<TKey>(in TKey key)
    {
        var keySerializer = GetSerializer<TKey>(0);

        var bytes = new byte[_prefix.Length + keySerializer.GetSerializedLength(in key)];

        var span = bytes.AsSpan();

        _prefix.CopyTo(span);

        keySerializer.Serialize(in key, span);

        return bytes;
    }

    public byte[] Build<TKey1, TKey2>(in TKey1 key1, in TKey2 key2)
    {
        var keySerializer1 = GetSerializer<TKey1>(0);
        var keySerializer2 = GetSerializer<TKey2>(1);

        var bytes = new byte[_prefix.Length + 
            keySerializer1.GetSerializedLength(in key1) + 
            keySerializer2.GetSerializedLength(in key2)];

        var span = bytes.AsSpan();

        _prefix.CopyTo(span);

        keySerializer1.Serialize(in key1, span);

        keySerializer2.Serialize(in key2, span);

        return bytes;
    }

    public byte[] Build<TKey1, TKey2, TKey3>(in TKey1 key1, in TKey2 key2, in TKey3 key3)
    {
        var keySerializer1 = GetSerializer<TKey1>(0);
        var keySerializer2 = GetSerializer<TKey2>(1);
        var keySerializer3 = GetSerializer<TKey3>(2);

        var bytes = new byte[_prefix.Length +
            keySerializer1.GetSerializedLength(in key1) +
            keySerializer2.GetSerializedLength(in key2) +
            keySerializer3.GetSerializedLength(in key3)];

        var span = bytes.AsSpan();

        _prefix.CopyTo(span);

        span = span.Slice(keySerializer1.Serialize(in key1, span));

        keySerializer2.Serialize(in key2, span);

        keySerializer3.Serialize(in key3, span);

        return bytes;
    }

    private IFixSerializer<TKey> GetSerializer<TKey>(int index)
    {
        return (IFixSerializer<TKey>)_serializers[index];
    }
}