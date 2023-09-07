namespace StackExchange.Redis.Entity;

public interface IRedisValueSerializer<T> : IRedisValueSerializer
{
    RedisValue Serialize(in T? value);
}