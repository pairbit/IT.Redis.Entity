namespace StackExchange.Redis.Entity.Formatters;

public class ByteArrayFormatter : IRedisValueFormatter<byte[]>
{
    public static readonly ByteArrayFormatter Default = new();

    public void Deserialize(in RedisValue redisValue, ref byte[]? value) => value = redisValue;

    public RedisValue Serialize(in byte[]? value) => value;
}