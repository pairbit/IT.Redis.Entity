using System.Collections.Specialized;
using System.Numerics;
using System.Xml.Linq;

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

        if (redisKey == null || redisKey.Length < length)
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
        throw new NotImplementedException();
    }
}