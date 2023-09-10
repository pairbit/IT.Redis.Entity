namespace StackExchange.Redis.Entity.Internal;

internal class RedisValueDeserializerProxy
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
}