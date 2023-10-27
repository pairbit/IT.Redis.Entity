namespace IT.Redis.Entity;

public class KeyBuilder : IKeyBuilder
{
    public static readonly KeyBuilder Default = new KeyBuilder(new Utf8FormatterVar());
    public static readonly KeyBuilder Fixed = new KeyBuilder(new Utf8FormatterFixed());
    public static readonly int MaxKeys = 8;

    private readonly byte _separator;
    private readonly IUtf8Formatter _utf8Formatter;

    public KeyBuilder(IUtf8Formatter utf8Formatter, byte separator = (byte)':')
    {
        _utf8Formatter = utf8Formatter ?? throw new ArgumentNullException(nameof(utf8Formatter));
        _separator = separator;
    }

    public byte[] BuildKey<TKey1>(byte[]? key, byte bits, in TKey1 key1)
    {
        var f = _utf8Formatter;
        var length = f.GetLength(in key1);
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            f.Format(in key1, key);
        }
        else if ((bits & 1) == 1)
        {
            f.Format(in key1, key);
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2)
    {
        var f = _utf8Formatter;
        var lenKey1 = f.GetLength(in key1);
        var length = 1 + lenKey1 + f.GetLength(in key2);
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            var offset = f.Format(in key1, key);
            key[offset++] = _separator;
            f.Format(in key2, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f.Format(in key1, key);
            if ((bits & 2) == 2) f.Format(in key2, key.AsSpan(lenKey1 + 1));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3)
    {
        var f = _utf8Formatter;
        var sep = _separator;
        var lenKey1 = f.GetLength(in key1);
        var lenKey2 = f.GetLength(in key2);
        var length = 2 + lenKey1 + lenKey2 + f.GetLength(in key3);
        var offset = 0;
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            offset += f.Format(in key1, key); key[offset++] = sep;
            offset += f.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
            f.Format(in key3, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f.Format(in key1, key);
            offset += lenKey1 + 1;
            if ((bits & 2) == 2) f.Format(in key2, key.AsSpan(offset));
            offset += lenKey2 + 1;
            if ((bits & 4) == 4) f.Format(in key3, key.AsSpan(offset));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4)
    {
        var f = _utf8Formatter;
        var sep = _separator;
        var lenKey1 = f.GetLength(in key1);
        var lenKey2 = f.GetLength(in key2);
        var lenKey3 = f.GetLength(in key3);
        var length = 3 + lenKey1 + lenKey2 + lenKey3
                       + f.GetLength(in key4);
        var offset = 0;
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            offset += f.Format(in key1, key); key[offset++] = sep;
            offset += f.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
            f.Format(in key4, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f.Format(in key1, key);
            offset += lenKey1 + 1;
            if ((bits & 2) == 2) f.Format(in key2, key.AsSpan(offset));
            offset += lenKey2 + 1;
            if ((bits & 4) == 4) f.Format(in key3, key.AsSpan(offset));
            offset += lenKey3 + 1;
            if ((bits & 8) == 8) f.Format(in key4, key.AsSpan(offset));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5)
    {
        var f = _utf8Formatter;
        var sep = _separator;
        var lenKey1 = f.GetLength(in key1);
        var lenKey2 = f.GetLength(in key2);
        var lenKey3 = f.GetLength(in key3);
        var lenKey4 = f.GetLength(in key4);
        var length = 4 + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + f.GetLength(in key5);
        var offset = 0;
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            offset += f.Format(in key1, key); key[offset++] = sep;
            offset += f.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key4, key.AsSpan(offset)); key[offset++] = sep;
            f.Format(in key5, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f.Format(in key1, key);
            offset += lenKey1 + 1;
            if ((bits & 2) == 2) f.Format(in key2, key.AsSpan(offset));
            offset += lenKey2 + 1;
            if ((bits & 4) == 4) f.Format(in key3, key.AsSpan(offset));
            offset += lenKey3 + 1;
            if ((bits & 8) == 8) f.Format(in key4, key.AsSpan(offset));
            offset += lenKey4 + 1;
            if ((bits & 16) == 16) f.Format(in key5, key.AsSpan(offset));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6)
    {
        var f = _utf8Formatter;
        var sep = _separator;
        var lenKey1 = f.GetLength(in key1);
        var lenKey2 = f.GetLength(in key2);
        var lenKey3 = f.GetLength(in key3);
        var lenKey4 = f.GetLength(in key4);
        var lenKey5 = f.GetLength(in key5);
        var length = 5 + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + lenKey5 + f.GetLength(in key6);
        var offset = 0;
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            offset += f.Format(in key1, key); key[offset++] = sep;
            offset += f.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key4, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key5, key.AsSpan(offset)); key[offset++] = sep;
            f.Format(in key6, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f.Format(in key1, key);
            offset += lenKey1 + 1;
            if ((bits & 2) == 2) f.Format(in key2, key.AsSpan(offset));
            offset += lenKey2 + 1;
            if ((bits & 4) == 4) f.Format(in key3, key.AsSpan(offset));
            offset += lenKey3 + 1;
            if ((bits & 8) == 8) f.Format(in key4, key.AsSpan(offset));
            offset += lenKey4 + 1;
            if ((bits & 16) == 16) f.Format(in key5, key.AsSpan(offset));
            offset += lenKey5 + 1;
            if ((bits & 32) == 32) f.Format(in key6, key.AsSpan(offset));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7)
    {
        var f = _utf8Formatter;
        var sep = _separator;
        var lenKey1 = f.GetLength(in key1);
        var lenKey2 = f.GetLength(in key2);
        var lenKey3 = f.GetLength(in key3);
        var lenKey4 = f.GetLength(in key4);
        var lenKey5 = f.GetLength(in key5);
        var lenKey6 = f.GetLength(in key6);
        var length = 6 + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + lenKey5 + lenKey6 + f.GetLength(in key7);
        var offset = 0;
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            offset += f.Format(in key1, key); key[offset++] = sep;
            offset += f.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key4, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key5, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key6, key.AsSpan(offset)); key[offset++] = sep;
            f.Format(in key7, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f.Format(in key1, key);
            offset += lenKey1 + 1;
            if ((bits & 2) == 2) f.Format(in key2, key.AsSpan(offset));
            offset += lenKey2 + 1;
            if ((bits & 4) == 4) f.Format(in key3, key.AsSpan(offset));
            offset += lenKey3 + 1;
            if ((bits & 8) == 8) f.Format(in key4, key.AsSpan(offset));
            offset += lenKey4 + 1;
            if ((bits & 16) == 16) f.Format(in key5, key.AsSpan(offset));
            offset += lenKey5 + 1;
            if ((bits & 32) == 32) f.Format(in key6, key.AsSpan(offset));
            offset += lenKey6 + 1;
            if ((bits & 64) == 64) f.Format(in key7, key.AsSpan(offset));
        }
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7, TKey8>(byte[]? key, byte bits, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7, in TKey8 key8)
    {
        var f = _utf8Formatter;
        var sep = _separator;
        var lenKey1 = f.GetLength(in key1);
        var lenKey2 = f.GetLength(in key2);
        var lenKey3 = f.GetLength(in key3);
        var lenKey4 = f.GetLength(in key4);
        var lenKey5 = f.GetLength(in key5);
        var lenKey6 = f.GetLength(in key6);
        var lenKey7 = f.GetLength(in key7);
        var length = 7 + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + lenKey5 + lenKey6 + lenKey7 + f.GetLength(in key8);
        var offset = 0;
        if (key == null || key.Length != length)
        {
            key = new byte[length];
            offset += f.Format(in key1, key); key[offset++] = sep;
            offset += f.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key4, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key5, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key6, key.AsSpan(offset)); key[offset++] = sep;
            offset += f.Format(in key7, key.AsSpan(offset)); key[offset++] = sep;
            f.Format(in key8, key.AsSpan(offset));
        }
        else
        {
            if ((bits & 1) == 1) f.Format(in key1, key);
            offset += lenKey1 + 1;
            if ((bits & 2) == 2) f.Format(in key2, key.AsSpan(offset));
            offset += lenKey2 + 1;
            if ((bits & 4) == 4) f.Format(in key3, key.AsSpan(offset));
            offset += lenKey3 + 1;
            if ((bits & 8) == 8) f.Format(in key4, key.AsSpan(offset));
            offset += lenKey4 + 1;
            if ((bits & 16) == 16) f.Format(in key5, key.AsSpan(offset));
            offset += lenKey5 + 1;
            if ((bits & 32) == 32) f.Format(in key6, key.AsSpan(offset));
            offset += lenKey6 + 1;
            if ((bits & 64) == 64) f.Format(in key7, key.AsSpan(offset));
            offset += lenKey7 + 1;
            if ((bits & 128) == 128) f.Format(in key8, key.AsSpan(offset));
        }
        return key;
    }
}