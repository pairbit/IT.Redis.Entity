namespace StackExchange.Redis.Entity;

public interface IRedisEntityReader<T>
{
    IRedisEntityFields Fields { get; }

    RedisValue Read(T entity, in RedisValue field);
}