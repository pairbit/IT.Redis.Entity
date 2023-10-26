namespace IT.Redis.Entity;

public class RedisKeyBuilder : IRedisKeyBuilder
{
    public static readonly RedisKeyBuilder Default = new(new Utf8FormatterDefault());
    public static readonly int MaxKeys = 8;
    private static readonly char SeparatorChar = ':';
    private static readonly byte Separator = (byte)SeparatorChar;

    private readonly IUtf8Formatter _utf8Formatter;

    public RedisKeyBuilder(IUtf8Formatter utf8Formatter)
    {
        _utf8Formatter = utf8Formatter ?? throw new ArgumentNullException(nameof(utf8Formatter));
    }

    public byte[] BuildKey<TKey>(byte[]? redisKey, byte bits, int offset, in TKey key)
    {
        var f = _utf8Formatter;
        var length = offset + f.GetLength(in key);

        if (redisKey == null || redisKey.Length != length)
            redisKey = new byte[length];

        if ((bits & 1) == 1) f.Format(in key, redisKey.AsSpan(offset));

        return redisKey;
    }

    public byte[] BuildKey<TKey1, TKey2>(byte[]? redisKey, byte bits, int offset, in TKey1 key1, in TKey2 key2)
    {
        throw new NotImplementedException();
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3>(byte[]? redisKey, byte bits, int offset, in TKey1 key1, in TKey2 key2, in TKey3 key3)
    {
        throw new NotImplementedException();
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4>(byte[]? redisKey, byte bits, int offset, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4)
    {
        throw new NotImplementedException();
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5>(byte[]? redisKey, byte bits, int offset, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5)
    {
        throw new NotImplementedException();
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6>(byte[]? redisKey, byte bits, int offset, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6)
    {
        throw new NotImplementedException();
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7>(byte[]? redisKey, byte bits, int offset, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7)
    {
        throw new NotImplementedException();
    }

    public byte[] BuildKey<TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7, TKey8>(byte[]? redisKey, byte bits, int offset, in TKey1 key1, in TKey2 key2, in TKey3 key3, in TKey4 key4, in TKey5 key5, in TKey6 key6, in TKey7 key7, in TKey8 key8)
    {
        var f = _utf8Formatter;
        var sep = Separator;
        var lenKey1 = f.GetLength(in key1);
        var lenKey2 = f.GetLength(in key2);
        var lenKey3 = f.GetLength(in key3);
        var lenKey4 = f.GetLength(in key4);
        var lenKey5 = f.GetLength(in key5);
        var lenKey6 = f.GetLength(in key6);
        var lenKey7 = f.GetLength(in key7);
        var length = 7 + offset + lenKey1 + lenKey2 + lenKey3 + lenKey4
                       + lenKey5 + lenKey6 + lenKey7 + f.GetLength(in key8);

        if (redisKey == null || redisKey.Length != length)
            redisKey = new byte[length];

        var span = redisKey.AsSpan(offset);

        if ((bits & 1) == 1) f.Format(in key1, span);

        offset += lenKey1;
        if ((bits & 2) == 2)
        {
            span[offset++] = sep;
            f.Format(in key2, span.Slice(offset));
        }
        else offset++;

        offset += lenKey2;
        if ((bits & 4) == 4)
        {
            span[offset++] = sep;
            f.Format(in key3, span.Slice(offset));
        }
        else offset++;

        offset += lenKey3;
        if ((bits & 8) == 8)
        {
            span[offset++] = sep;
            f.Format(in key4, span.Slice(offset));
        }
        else offset++;

        offset += lenKey4;
        if ((bits & 16) == 16)
        {
            span[offset++] = sep;
            f.Format(in key5, span.Slice(offset));
        }
        else offset++;

        offset += lenKey5;
        if ((bits & 32) == 32)
        {
            span[offset++] = sep;
            f.Format(in key6, span.Slice(offset));
        }
        else offset++;

        offset += lenKey6;
        if ((bits & 64) == 64)
        {
            span[offset++] = sep;
            f.Format(in key7, span.Slice(offset));
        }
        else offset++;

        offset += lenKey7;
        if ((bits & 128) == 128)
        {
            span[offset++] = sep;
            f.Format(in key8, span.Slice(offset));
        }

        return redisKey;
    }
}