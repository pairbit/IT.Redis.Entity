namespace IT.Redis.Entity.Extensions;

public static class xIDatabase
{
    #region ReadKey

    public static void EntitySet<TEntity>(this IDatabase db, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        db.HashSet(re.ReadKey(entity), re.ReadFields.GetEntries(entity), flags);
    }

    public static bool EntitySet<TEntity>(this IDatabase db, TEntity entity, RedisEntityField<TEntity> field, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSet(field.ReadKey(entity), field.ForRedis, field.Read(entity), when, flags);

    public static bool EntityLoad<TEntity>(this IDatabase db, TEntity entity, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
        => field.Write(entity, db.HashGet(field.ReadKey(entity), field.ForRedis, flags));

    public static bool EntityLoad<TEntity>(this IDatabase db, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.WriteFields;
        return fields.Write(entity, db.HashGet(re.ReadKey(entity), fields.ForRedis, flags));
    }

    #endregion ReadKey

    public static void EntitySet<TEntity>(this IDatabase db, in RedisKey key, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, (re ?? RedisEntity<TEntity>.Default).ReadFields.GetEntries(entity), flags);

    public static bool EntitySet<TEntity>(this IDatabase db, in RedisKey key, TEntity entity, RedisEntityField<TEntity> field, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, field.ForRedis, field.Read(entity), when, flags);

    public static bool EntitySetField<TEntity, TField>(this IDatabase db, in RedisKey key, RedisEntityField<TEntity> field, in TField value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, field.ForRedis, field.GetFormatter<TField>().Serialize(in value), when, flags);

    public static bool EntityLoad<TEntity>(this IDatabase db, in RedisKey key, TEntity entity, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
        => field.Write(entity, db.HashGet(key, field.ForRedis, flags));

    public static bool EntityLoad<TEntity>(this IDatabase db, in RedisKey key, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.WriteFields;
        return fields.Write(entity, db.HashGet(key, fields.ForRedis, flags));
    }

    public static bool EntityLoadField<TEntity, TField>(this IDatabase db, in RedisKey key, ref TField? value, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
    {
        var redisValue = db.HashGet(key, field.ForRedis, flags);

        if (redisValue.IsNull) return false;

        field.GetFormatter<TField>().Deserialize(in redisValue, ref value);

        return true;
    }

    public static TEntity? EntityGet<TEntity, IEntity>(this IDatabase db, in RedisKey key, RedisEntityField<IEntity> field, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
        => field.GetEntity<TEntity, IEntity>(db.HashGet(key, field.ForRedis, flags));

    public static TEntity? EntityGet<TEntity, IEntity>(this IDatabase db, in RedisKey key, RedisEntity<IEntity>? re = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
    {
        re ??= RedisEntity<IEntity>.Default;
        var fields = re.WriteFields;
        return fields.GetEntity<TEntity, IEntity>(db.HashGet(key, fields.ForRedis, flags));
    }

    public static TEntity? EntityGet<TEntity>(this IDatabase db, in RedisKey key, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None) where TEntity : new()
        => field.GetEntity<TEntity, TEntity>(db.HashGet(key, field.ForRedis, flags));

    public static TEntity? EntityGet<TEntity>(this IDatabase db, in RedisKey key, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None) where TEntity : new()
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.WriteFields;
        return fields.GetEntity<TEntity, TEntity>(db.HashGet(key, fields.ForRedis, flags));
    }

    //public static ExistsValue<TField?> EntityGetField<TEntity, TField>(this IDatabase db, in RedisKey key, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
    //{
    //    var redisValue = db.HashGet(key, field.ForRedis, flags);
    //    if (redisValue.IsNull) return default;

    //    TField? value = default;
    //    field.GetFormatter<TField>().Deserialize(in redisValue, ref value);
    //    return new(value);
    //}
}