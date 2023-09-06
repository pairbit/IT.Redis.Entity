namespace StackExchange.Redis.Entity;

public interface IRedisEntityWriter<T>
{
    IRedisEntityFields Fields { get; }

    bool Write(T entity, in RedisValue field, in RedisValue value);
}