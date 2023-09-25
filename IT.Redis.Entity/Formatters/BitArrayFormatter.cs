using IT.Redis.Entity.Internal;
using System.Collections;

namespace IT.Redis.Entity.Formatters;

public class BitArrayFormatter : IRedisValueFormatter<BitArray>
{
    private static readonly BitArray Empty = new(Array.Empty<int>());

    public static readonly BitArrayFormatter Default = new();


    public void Deserialize(in RedisValue redisValue, ref BitArray? value)
    {
        if (redisValue == RedisValues.Zero)
        {
            value = null;
        }
        else if (redisValue == RedisValue.EmptyString)
        {
            value = Empty;
        }
        else
        {
            value = new BitArray((byte[])redisValue!);
        }
    }

    public RedisValue Serialize(in BitArray? value)
    {
        if (value == null) return RedisValues.Zero;
        if (value.Length == 0) return RedisValue.EmptyString;

        var bytes = new byte[(value.Length - 1) / 8 + 1];

        value.CopyTo(bytes, 0);

        return bytes;
    }
}