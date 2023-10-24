namespace IT.Redis.Entity.Internal;

internal class KeyBuilder
{
    public static readonly int MaxKeys = 5;
    private readonly List<object> _serializers = new(MaxKeys);
    private readonly byte[] _prefix;
    private readonly byte _separator = (byte)':';

    public KeyBuilder(byte[]? prefix)
    {
        _prefix = prefix ?? Array.Empty<byte>();
    }

    public void AddSerializer(object serializer)
    {
        if (_serializers.Count == MaxKeys) throw new InvalidOperationException($"A composite key containing more than {MaxKeys} fields is not supported");

        _serializers.Add(serializer);
    }

    public byte[] Build<TKey>(in TKey key)
    {
        var f = GetFormatter<TKey>(0);

        var prefix = _prefix;
        var offset = prefix.Length;

        var bytes = new byte[offset + f.GetLength(in key)];
        var span = bytes.AsSpan();

        if (offset > 0)
        {
            prefix.CopyTo(span);
            span = span.Slice(offset);
        }

        f.Format(in key, span);

        return bytes;
    }

    public byte[] Build<TKey1, TKey2>(in TKey1 key1, in TKey2 key2)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);

        var sep = _separator;
        var prefix = _prefix;
        var offset = prefix.Length;

        var bytes = new byte[1 + offset +
            f1.GetLength(in key1) +
            f2.GetLength(in key2)];

        var span = bytes.AsSpan();

        if (offset > 0)
        {
            prefix.CopyTo(span);
            span = span.Slice(offset);
            offset = 0;
        }

        offset += f1.Format(in key1, span);
        span[offset++] = sep;
        f2.Format(in key2, span.Slice(offset));

        return bytes;
    }

    public byte[] Build<TKey1, TKey2, TKey3>(in TKey1 key1, in TKey2 key2, in TKey3 key3)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);

        var sep = _separator;
        var prefix = _prefix;
        var offset = prefix.Length;

        var bytes = new byte[2 + offset +
            f1.GetLength(in key1) +
            f2.GetLength(in key2) +
            f3.GetLength(in key3)];

        var span = bytes.AsSpan();

        if (offset > 0)
        {
            prefix.CopyTo(span);
            span = span.Slice(offset);
            offset = 0;
        }

        offset += f1.Format(in key1, span);
        span[offset++] = sep;
        offset += f2.Format(in key2, span.Slice(offset));
        span[offset++] = sep;
        f3.Format(in key3, span.Slice(offset));

        return bytes;
    }

    public byte[] Build<TKey1, TKey2, TKey3, TKey4>(in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var f4 = GetFormatter<TKey4>(3);

        var sep = _separator;
        var prefix = _prefix;
        var offset = prefix.Length;

        var bytes = new byte[3 + offset +
            f1.GetLength(in key1) +
            f2.GetLength(in key2) +
            f3.GetLength(in key3) +
            f4.GetLength(in key4)];

        var span = bytes.AsSpan();

        if (offset > 0)
        {
            prefix.CopyTo(span);
            span = span.Slice(offset);
            offset = 0;
        }

        offset += f1.Format(in key1, span);
        span[offset++] = sep;
        offset += f2.Format(in key2, span.Slice(offset));
        span[offset++] = sep;
        offset += f3.Format(in key3, span.Slice(offset));
        span[offset++] = sep;
        f4.Format(in key4, span.Slice(offset));

        return bytes;
    }

    public byte[] Build<TKey1, TKey2, TKey3, TKey4, TKey5>(in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var f4 = GetFormatter<TKey4>(3);
        var f5 = GetFormatter<TKey5>(4);

        var sep = _separator;
        var prefix = _prefix;
        var offset = prefix.Length;

        var bytes = new byte[4 + offset +
            f1.GetLength(in key1) +
            f2.GetLength(in key2) +
            f3.GetLength(in key3) +
            f4.GetLength(in key4) +
            f5.GetLength(in key5)];

        var span = bytes.AsSpan();

        if (offset > 0)
        {
            prefix.CopyTo(span);
            span = span.Slice(offset);
            offset = 0;
        }

        offset += f1.Format(in key1, span);
        span[offset++] = sep;
        offset += f2.Format(in key2, span.Slice(offset));
        span[offset++] = sep;
        offset += f3.Format(in key3, span.Slice(offset));
        span[offset++] = sep;
        offset += f4.Format(in key4, span.Slice(offset));
        span[offset++] = sep;
        f5.Format(in key5, span.Slice(offset));

        return bytes;
    }

    private IUtf8Formatter<TKey> GetFormatter<TKey>(int index)
    {
        return (IUtf8Formatter<TKey>)_serializers[index];
    }
}