namespace IT.Redis.Entity;

public interface IRedisEntityFactory
{
    IRedisEntity<TEntity> New<TEntity>();
}