namespace StackExchange.Redis.Entity;

public interface IRedisValueSerializer
{
    RedisValue Serialize<T>(in T? value);
}