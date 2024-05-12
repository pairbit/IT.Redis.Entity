namespace IT.Redis.Entity;

public interface IRedisEntityFactory
{
    RedisEntity<TEntity> New<TEntity>();
}