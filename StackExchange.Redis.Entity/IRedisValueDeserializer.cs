namespace StackExchange.Redis.Entity;

public interface IRedisValueDeserializer
{
    void Deserialize(in RedisValue redisValue, ref object? value);
}