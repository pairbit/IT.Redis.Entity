namespace IT.Redis.Entity.Extensions;

public static class xIRedisEntityField
{
    public static TEntity? GetEntity<TEntity, IEntity>(this IRedisEntityField<IEntity> field, in RedisValue value) where TEntity : IEntity, new()
    {
        if (value.IsNull) return default;

        var entity = new TEntity();

        field.Write(entity, in value);

        return entity;
    }

    public static TEntity? GetEntity<TEntity>(this IRedisEntityField<TEntity> field, in RedisValue value) where TEntity : new()
        => field.GetEntity<TEntity, TEntity>(in value);
}