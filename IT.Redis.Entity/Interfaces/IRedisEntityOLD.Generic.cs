namespace IT.Redis.Entity;

public interface IRedisEntityOLD<T>
{
    IKeyBuilder KeyBuilder { get; }

    RedisKey ReadKey(T entity);
}