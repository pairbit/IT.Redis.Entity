namespace StackExchange.Redis.Entity;

public interface IRedisEntityWriter<T>
{
    RedisValue[] Fields { get; }

    void Write(T entity, RedisValue field, RedisValue value);
}