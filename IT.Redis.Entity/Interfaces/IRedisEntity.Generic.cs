namespace IT.Redis.Entity;

public interface IRedisEntity<T>
{
    IKeyBuilder KeyBuilder { get; }

    RedisKey ReadKey(T entity);
}