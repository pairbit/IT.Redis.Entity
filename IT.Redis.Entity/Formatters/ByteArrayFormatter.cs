namespace IT.Redis.Entity.Formatters;

public class ByteArrayFormatter : IRedisValueFormatter<byte[]?>
{
    public static readonly ByteArrayFormatter Default = new();

    public void Deserialize(in RedisValue redisValue, ref byte[]? value)
    {
        if (redisValue.IsNull)
        {
            value = null;
        }
        else if (redisValue.IsNullOrEmpty)
        {
            value = Array.Empty<byte>();
        }
        else
        {
            value = redisValue;
        }
    }

    public RedisValue Serialize(in byte[]? value)
        => value == null ? RedisValue.EmptyString : value;
}