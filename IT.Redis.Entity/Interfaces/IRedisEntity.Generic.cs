namespace IT.Redis.Entity;

public interface IRedisEntity<TEntity>
{
    IKeyRebuilder KeyBuilder { get; }

    RedisKey ReadKey(TEntity entity);

    RedisEntityFields<TEntity> Fields { get; }

    RedisEntityFields<TEntity> ReadFields { get; }

    RedisEntityFields<TEntity> WriteFields { get; }

    RedisEntityFields<TEntity> AllFields { get; }
}