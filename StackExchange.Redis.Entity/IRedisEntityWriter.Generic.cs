namespace StackExchange.Redis.Entity;

public interface IRedisEntityWriter<T>
{
    IRedisEntityFields Fields { get; }

    void Write(T entity, in RedisValue field, in RedisValue value);
}