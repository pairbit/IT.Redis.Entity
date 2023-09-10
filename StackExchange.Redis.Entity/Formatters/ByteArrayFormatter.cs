using StackExchange.Redis.Entity.Internal;

namespace StackExchange.Redis.Entity.Formatters;

public class ByteArrayFormatter : IRedisValueFormatter<byte[]>
{
    public static readonly ByteArrayFormatter Default = new();

    public void Deserialize(in RedisValue redisValue, ref byte[]? value) 
        => value = redisValue == RedisValues.Null ? (byte[]?)null : redisValue;

    public RedisValue Serialize(in byte[]? value) 
        => value == null ? RedisValues.Null : value;
}