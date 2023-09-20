namespace StackExchange.Redis.Entity.Internal;

internal class RedisValueSerializerProxy<T> : IRedisValueSerializer<T>
{
    private readonly IRedisValueSerializer _serializer;

    public RedisValueSerializerProxy(IRedisValueSerializer serializer)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public RedisValue Serialize(in T? value) => _serializer.Serialize(in value);
}