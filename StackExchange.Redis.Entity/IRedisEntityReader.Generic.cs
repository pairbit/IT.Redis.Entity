namespace StackExchange.Redis.Entity;

public interface IRedisEntityReader<T>
{
    RedisValue[] Fields { get; }

    RedisValue Read(T entity, RedisValue field);
}