namespace StackExchange.Redis.Entity.Internal;

internal class RedisValueDeserializerProxy<T> : IRedisValueDeserializer<T>
{
    private readonly IRedisValueDeserializer _deserializer;

    public RedisValueDeserializerProxy(IRedisValueDeserializer deserializer)
    {
        _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
    }

    public void Deserialize(in RedisValue redisValue, ref T? value) => _deserializer.Deserialize(in redisValue, ref value);
}