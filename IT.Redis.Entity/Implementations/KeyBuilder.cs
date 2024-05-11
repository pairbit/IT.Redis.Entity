using IT.Redis.Entity.Extensions;
using IT.Redis.Entity.Utf8Formatters;

namespace IT.Redis.Entity;

public class KeyBuilder : IKeyBuilder
{
    public static readonly KeyBuilder Default = new(Utf8FormatterVar.Default);
    public static readonly KeyBuilder Fixed = new(Utf8FormatterFixed.Default);

    protected readonly byte _separator;
    protected readonly IUtf8Formatter _utf8Formatter;

    public KeyBuilder(IUtf8Formatter utf8Formatter, byte separator = (byte)':')
    {
        _utf8Formatter = utf8Formatter ?? throw new ArgumentNullException(nameof(utf8Formatter));
        _separator = separator;
    }

    public byte[] BuildKey<TKey1>(in TKey1 key1)
    {
        var f = _utf8Formatter;
        var key = new byte[f.GetLength(in key1)];
        f.Format(in key1, key);
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2>(in TKey1 key1, in TKey2 key2)
    {
        var f = _utf8Formatter;
        var key = new byte[1 + f.GetLength(in key1) + f.GetLength(in key2)];
        var offset = f.Format(in key1, key);
        key[offset++] = _separator;
        f.Format(in key2, key.AsSpan(offset));
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3>(in TKey1 key1, in TKey2 key2, in TKey3 key3)
    {
        var f = _utf8Formatter;
        var key = new byte[2 + f.GetLength(in key1) + f.GetLength(in key2) + f.GetLength(in key3)];
        var sep = _separator;
        var offset = f.Format(in key1, key); key[offset++] = sep;
        offset += f.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
        f.Format(in key3, key.AsSpan(offset));
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4>(in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4)
    {
        var f = _utf8Formatter;
        var key = new byte[3 + f.GetLength(in key1) + f.GetLength(in key2)
                             + f.GetLength(in key3) + f.GetLength(in key4)];
        var sep = _separator;
        var offset = f.Format(in key1, key); key[offset++] = sep;
        offset += f.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
        f.Format(in key4, key.AsSpan(offset));
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5>(in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5)
    {
        var f = _utf8Formatter;
        var key = new byte[4 + f.GetLength(in key1) + f.GetLength(in key2) + f.GetLength(in key3)
                             + f.GetLength(in key4) + f.GetLength(in key5)];
        var sep = _separator;
        var offset = f.Format(in key1, key); key[offset++] = sep;
        offset += f.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key4, key.AsSpan(offset)); key[offset++] = sep;
        f.Format(in key5, key.AsSpan(offset));
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6>(in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6)
    {
        var f = _utf8Formatter;
        var key = new byte[5 + f.GetLength(in key1) + f.GetLength(in key2) + f.GetLength(in key3)
                             + f.GetLength(in key4) + f.GetLength(in key5) + f.GetLength(in key6)];
        var sep = _separator;
        var offset = f.Format(in key1, key); key[offset++] = sep;
        offset += f.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key4, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key5, key.AsSpan(offset)); key[offset++] = sep;
        f.Format(in key6, key.AsSpan(offset));
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7>(in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7)
    {
        var f = _utf8Formatter;
        var key = new byte[6 + f.GetLength(in key1) + f.GetLength(in key2) + f.GetLength(in key3)
                             + f.GetLength(in key4) + f.GetLength(in key5) + f.GetLength(in key6)
                             + f.GetLength(in key7)];
        var sep = _separator;
        var offset = f.Format(in key1, key); key[offset++] = sep;
        offset += f.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key4, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key5, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key6, key.AsSpan(offset)); key[offset++] = sep;
        f.Format(in key7, key.AsSpan(offset));
        return key;
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7, TKey8>(in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7, in TKey8 key8)
    {
        var f = _utf8Formatter;
        var key = new byte[7 + f.GetLength(in key1) + f.GetLength(in key2) + f.GetLength(in key3)
                             + f.GetLength(in key4) + f.GetLength(in key5) + f.GetLength(in key6)
                             + f.GetLength(in key7) + f.GetLength(in key8)];
        var sep = _separator;
        var offset = f.Format(in key1, key); key[offset++] = sep;
        offset += f.Format(in key2, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key3, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key4, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key5, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key6, key.AsSpan(offset)); key[offset++] = sep;
        offset += f.Format(in key7, key.AsSpan(offset)); key[offset++] = sep;
        f.Format(in key8, key.AsSpan(offset));
        return key;
    }
}