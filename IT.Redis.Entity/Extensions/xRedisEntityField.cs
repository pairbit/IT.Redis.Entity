namespace IT.Redis.Entity.Extensions;

public static class xRedisEntityField
{
    public static TEntity? GetEntity<TEntity, IEntity>(this RedisEntityField<IEntity> field, in RedisValue value) where TEntity : IEntity, new()
    {
        if (value.IsNull) return default;

        var entity = new TEntity();

        field.Write(entity, in value);

        return entity;
    }

    public static TEntity? GetEntity<TEntity>(this RedisEntityField<TEntity> field, in RedisValue value) where TEntity : new()
        => field.GetEntity<TEntity, TEntity>(in value);

    public static TEntity? GetEntity<TEntity, IEntity>(this RedisEntityField<IEntity> field, in RedisValue value, Func<TEntity> newEntity) where TEntity : IEntity
    {
        if (value.IsNull) return default;

        var entity = newEntity();

        field.Write(entity, in value);

        return entity;
    }

    public static TEntity? GetEntity<TEntity>(this RedisEntityField<TEntity> field, in RedisValue value, Func<TEntity> newEntity)
        => field.GetEntity<TEntity, TEntity>(in value, newEntity);
}