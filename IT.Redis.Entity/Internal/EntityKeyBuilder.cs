using System.Text;

namespace IT.Redis.Entity.Internal;

internal class EntityKeyBuilder : IKeyBuilder
{
    public static readonly int MaxKeys = 5;
    private static readonly char _separatorChar = ':';
    private static readonly byte _separator = (byte)_separatorChar;

    private readonly List<object> _serializers = new(MaxKeys);
    private readonly byte[] _prefix;

    public EntityKeyBuilder(string? prefix)
    {
        _prefix = GetPrefix(prefix);
    }

    public void AddSerializer(object serializer)
    {
        if (_serializers.Count == MaxKeys) throw new InvalidOperationException($"A composite key containing more than {MaxKeys} fields is not supported");

        _serializers.Add(serializer);
    }

    public byte[] BuildKey<TKey1>(byte[]? key, byte bits, in TKey1 key1)
    {
        var f1 = GetFormatter<TKey1>(0);
        var prefix = _prefix;
        var offset = prefix.Length;
        var length = offset + f1.GetLength(in key1);

        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            bits = 255;
        }

        if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));

        return key;
    }

    public byte[] BuildKey<TKey1, TKey2>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var prefix = _prefix;
        var offset = prefix.Length;
        var lenKey1 = f1.GetLength(in key1);
        var length = 1 + offset + lenKey1 + f2.GetLength(in key2);

        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            bits = 255;
        }

        if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));

        if ((bits & 2) == 2)
        {
            offset += lenKey1;
            key[offset++] = _separator;
            f2.Format(in key2, key.AsSpan(offset));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var prefix = _prefix;
        var offset = prefix.Length;
        var sep = _separator;
        var lenKey1 = f1.GetLength(in key1);
        var lenKey2 = f2.GetLength(in key2);
        var length = 2 + offset + lenKey1 + lenKey2 + f3.GetLength(in key3);

        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            bits = 255;
        }

        if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));

        offset += lenKey1;
        if ((bits & 2) == 2)
        {
            key[offset++] = sep;
            f2.Format(in key2, key.AsSpan(offset));
        }
        else offset++;

        if ((bits & 4) == 4)
        {
            offset += lenKey2;
            key[offset++] = sep;
            f3.Format(in key3, key.AsSpan(offset));
        }

        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var f4 = GetFormatter<TKey4>(3);
        var prefix = _prefix;
        var offset = prefix.Length;
        var sep = _separator;
        var lenKey1 = f1.GetLength(in key1);
        var lenKey2 = f2.GetLength(in key2);
        var lenKey3 = f3.GetLength(in key3);
        var length = 3 + offset + lenKey1 + lenKey2 + lenKey3
                       + f4.GetLength(in key4);

        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            bits = 255;
        }

        if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));

        offset += lenKey1;
        if ((bits & 2) == 2)
        {
            key[offset++] = sep;
            f2.Format(in key2, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey2;
        if ((bits & 4) == 4)
        {
            key[offset++] = sep;
            f3.Format(in key3, key.AsSpan(offset));
        }
        else offset++;

        if ((bits & 8) == 8)
        {
            offset += lenKey3;
            key[offset++] = sep;
            f4.Format(in key4, key.AsSpan(offset));
        }

        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var f4 = GetFormatter<TKey4>(3);
        var f5 = GetFormatter<TKey5>(4);
        var prefix = _prefix;
        var offset = prefix.Length;
        var sep = _separator;
        var lenKey1 = f1.GetLength(in key1);
        var lenKey2 = f2.GetLength(in key2);
        var lenKey3 = f3.GetLength(in key3);
        var lenKey4 = f4.GetLength(in key4);
        var length = 4 + offset + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + f5.GetLength(in key5);

        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            bits = 255;
        }

        if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));

        offset += lenKey1;
        if ((bits & 2) == 2)
        {
            key[offset++] = sep;
            f2.Format(in key2, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey2;
        if ((bits & 4) == 4)
        {
            key[offset++] = sep;
            f3.Format(in key3, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey3;
        if ((bits & 8) == 8)
        {
            key[offset++] = sep;
            f4.Format(in key4, key.AsSpan(offset));
        }
        else offset++;

        if ((bits & 16) == 16)
        {
            offset += lenKey4;
            key[offset++] = sep;
            f5.Format(in key5, key.AsSpan(offset));
        }

        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var f4 = GetFormatter<TKey4>(3);
        var f5 = GetFormatter<TKey5>(4);
        var f6 = GetFormatter<TKey6>(5);
        var prefix = _prefix;
        var offset = prefix.Length;
        var sep = _separator;
        var lenKey1 = f1.GetLength(in key1);
        var lenKey2 = f2.GetLength(in key2);
        var lenKey3 = f3.GetLength(in key3);
        var lenKey4 = f4.GetLength(in key4);
        var lenKey5 = f5.GetLength(in key5);
        var length = 5 + offset + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + lenKey5 + f6.GetLength(in key6);

        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            bits = 255;
        }

        if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));

        offset += lenKey1;
        if ((bits & 2) == 2)
        {
            key[offset++] = sep;
            f2.Format(in key2, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey2;
        if ((bits & 4) == 4)
        {
            key[offset++] = sep;
            f3.Format(in key3, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey3;
        if ((bits & 8) == 8)
        {
            key[offset++] = sep;
            f4.Format(in key4, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey4;
        if ((bits & 16) == 16)
        {
            key[offset++] = sep;
            f5.Format(in key5, key.AsSpan(offset));
        }
        else offset++;

        if ((bits & 32) == 32)
        {
            offset += lenKey5;
            key[offset++] = sep;
            f6.Format(in key6, key.AsSpan(offset));
        }

        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var f4 = GetFormatter<TKey4>(3);
        var f5 = GetFormatter<TKey5>(4);
        var f6 = GetFormatter<TKey6>(5);
        var f7 = GetFormatter<TKey7>(6);
        var prefix = _prefix;
        var offset = prefix.Length;
        var sep = _separator;
        var lenKey1 = f1.GetLength(in key1);
        var lenKey2 = f2.GetLength(in key2);
        var lenKey3 = f3.GetLength(in key3);
        var lenKey4 = f4.GetLength(in key4);
        var lenKey5 = f5.GetLength(in key5);
        var lenKey6 = f6.GetLength(in key6);
        var length = 6 + offset + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + lenKey5 + lenKey6 + f7.GetLength(in key7);

        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            bits = 255;
        }

        if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));

        offset += lenKey1;
        if ((bits & 2) == 2)
        {
            key[offset++] = sep;
            f2.Format(in key2, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey2;
        if ((bits & 4) == 4)
        {
            key[offset++] = sep;
            f3.Format(in key3, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey3;
        if ((bits & 8) == 8)
        {
            key[offset++] = sep;
            f4.Format(in key4, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey4;
        if ((bits & 16) == 16)
        {
            key[offset++] = sep;
            f5.Format(in key5, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey5;
        if ((bits & 32) == 32)
        {
            key[offset++] = sep;
            f6.Format(in key6, key.AsSpan(offset));
        }
        else offset++;

        if ((bits & 64) == 64)
        {
            offset += lenKey6;
            key[offset++] = sep;
            f7.Format(in key7, key.AsSpan(offset));
        }

        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7, TKey8>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7, in TKey8 key8)
    {
        var f1 = GetFormatter<TKey1>(0);
        var f2 = GetFormatter<TKey2>(1);
        var f3 = GetFormatter<TKey3>(2);
        var f4 = GetFormatter<TKey4>(3);
        var f5 = GetFormatter<TKey5>(4);
        var f6 = GetFormatter<TKey6>(5);
        var f7 = GetFormatter<TKey7>(6);
        var f8 = GetFormatter<TKey8>(7);
        var prefix = _prefix;
        var offset = prefix.Length;
        var sep = _separator;
        var lenKey1 = f1.GetLength(in key1);
        var lenKey2 = f2.GetLength(in key2);
        var lenKey3 = f3.GetLength(in key3);
        var lenKey4 = f4.GetLength(in key4);
        var lenKey5 = f5.GetLength(in key5);
        var lenKey6 = f6.GetLength(in key6);
        var lenKey7 = f7.GetLength(in key7);
        var length = 7 + offset + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + lenKey5 + lenKey6 + lenKey7 + f8.GetLength(in key8);

        if (key == null || key.Length != length)
        {
            key = new byte[length];
            if (offset > 0) prefix.CopyTo(key.AsSpan());
            bits = 255;
        }

        if ((bits & 1) == 1) f1.Format(in key1, key.AsSpan(offset));

        offset += lenKey1;
        if ((bits & 2) == 2)
        {
            key[offset++] = sep;
            f2.Format(in key2, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey2;
        if ((bits & 4) == 4)
        {
            key[offset++] = sep;
            f3.Format(in key3, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey3;
        if ((bits & 8) == 8)
        {
            key[offset++] = sep;
            f4.Format(in key4, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey4;
        if ((bits & 16) == 16)
        {
            key[offset++] = sep;
            f5.Format(in key5, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey5;
        if ((bits & 32) == 32)
        {
            key[offset++] = sep;
            f6.Format(in key6, key.AsSpan(offset));
        }
        else offset++;

        offset += lenKey6;
        if ((bits & 64) == 64)
        {
            key[offset++] = sep;
            f7.Format(in key7, key.AsSpan(offset));
        }
        else offset++;

        if ((bits & 128) == 128)
        {
            offset += lenKey7;
            key[offset++] = sep;
            f8.Format(in key8, key.AsSpan(offset));
        }

        return key;
    }

    /*
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
    */

    private IUtf8Formatter<TKey> GetFormatter<TKey>(int index)
    {
        return (IUtf8Formatter<TKey>)_serializers[index];
    }

    private static byte[] GetPrefix(string? prefix)
    {
        if (prefix == null) return Array.Empty<byte>();

        var encoding = Encoding.UTF8;
        var length = prefix.Length;

        if (prefix[length - 1] == _separatorChar)
            return encoding.GetBytes(prefix);

        var bytes = new byte[length + 1];

        encoding.GetBytes(prefix, bytes);

        bytes[length] = _separator;

        return bytes;
    }
}