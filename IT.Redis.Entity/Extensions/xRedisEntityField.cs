namespace IT.Redis.Entity.Extensions;

public static class xRedisEntityField
{
    public static TEntity? GetEntity<TEntity, IEntity>(this RedisEntityField<IEntity> field, in RedisValue value, Func<TEntity> getEntity) where TEntity : IEntity
    {
        if (value.IsNull) return default;

        var entity = getEntity();

        field.Write(entity, in value);

        return entity;
    }

    public static TEntity? GetEntity<TEntity>(this RedisEntityField<TEntity> field, in RedisValue value, Func<TEntity> getEntity)
        => field.GetEntity<TEntity, TEntity>(in value, getEntity);

    public static TEntity? GetEntity<TEntity, IEntity>(this RedisEntityField<IEntity> field, in RedisValue value) where TEntity : IEntity, new()
        => field.GetEntity(in value, static () => new TEntity());

    public static TEntity? GetEntity<TEntity>(this RedisEntityField<TEntity> field, in RedisValue value) where TEntity : new()
        => field.GetEntity<TEntity, TEntity>(in value);
}