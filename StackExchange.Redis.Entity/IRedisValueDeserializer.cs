namespace StackExchange.Redis.Entity;

public interface IRedisValueDeserializer
{
    void Deserialize<T>(in RedisValue redisValue, ref T? value);
}