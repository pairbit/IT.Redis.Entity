namespace IT.Redis.Entity;

public static class xIRedisEntity
{
    public static HashEntry[] GetEntries<TEntity>(this IRedisEntity<TEntity> redisEntity, TEntity entity) => redisEntity.Fields.GetEntries(entity);
}