namespace StackExchange.Redis.Entity;

public interface IRedisValueSerializer<T>
{
    RedisValue Serialize(in T? value);
}