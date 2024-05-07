namespace IT.Redis.Entity;

public interface IRedisEntity<TEntity>
{
    IKeyBuilder KeyBuilder { get; }

    RedisKey ReadKey(TEntity entity);

    RedisEntityFields<TEntity> Fields { get; }
}