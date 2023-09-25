using IT.Redis.Entity.Internal;

namespace IT.Redis.Entity.Formatters;

public class ByteArrayFormatter : IRedisValueFormatter<byte[]>
{
    public static readonly ByteArrayFormatter Default = new();

    public void Deserialize(in RedisValue redisValue, ref byte[]? value) 
        => value = redisValue == RedisValues.Zero ? (byte[]?)null : redisValue;

    public RedisValue Serialize(in byte[]? value)
        => value == null ? RedisValues.Zero : value;
}