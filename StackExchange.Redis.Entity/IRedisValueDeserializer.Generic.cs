namespace StackExchange.Redis.Entity;

public interface IRedisValueDeserializer<T> : IRedisValueDeserializer
{
    void Deserialize(in RedisValue redisValue, ref T? value);

    T? Deserialize(in RedisValue redisValue);
}