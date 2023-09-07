namespace StackExchange.Redis.Entity;

public abstract class Serializer<T> : IRedisValueSerializer<T>, IRedisValueDeserializer<T>
{
    public abstract void Deserialize(in RedisValue redisValue, ref T? value);

    public T? Deserialize(in RedisValue redisValue)
    {
        T? value = default;
        Deserialize(in redisValue, ref value);
        return value;
    }

    public void Deserialize(in RedisValue redisValue, ref object? value)
    {
        var cast = (T?)value;
        Deserialize(redisValue, ref cast);
    }

    public abstract RedisValue Serialize(in T? value);

    public RedisValue Serialize(object? value) => Serialize((T?)value);
}