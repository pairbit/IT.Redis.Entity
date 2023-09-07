namespace StackExchange.Redis.Entity;

public interface IRedisValueSerializer
{
    RedisValue Serialize(object? value);
}