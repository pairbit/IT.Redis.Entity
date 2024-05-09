namespace IT.Redis.Entity.Internal;

public class RedisValueDeserializerProxy
{
    private readonly IRedisValueDeserializer _deserializer;

    public RedisValueDeserializerProxy(IRedisValueDeserializer deserializer)
    {
        _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
    }

    public T? Deserialize<T>(in RedisValue redisValue, T? value)
    {
        _deserializer.Deserialize(in redisValue, ref value);
        return value;
    }

    public T? DeserializeNew<T>(in RedisValue redisValue)
    {
        T? value = default;
        _deserializer.Deserialize(in redisValue, ref value);
        return value;
    }
}