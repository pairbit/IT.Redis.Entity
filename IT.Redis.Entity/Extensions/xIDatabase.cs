namespace IT.Redis.Entity.Extensions;

public static class xIDatabase
{
    #region ReadKey

    public static void EntitySet<TEntity>(this IDatabase db, TEntity entity, IRedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        db.HashSet(re.ReadKey(entity), re.Fields.GetEntries(entity), flags);
    }

    public static void EntitySet<TEntity>(this IDatabase db, TEntity entity, RedisEntityFields<TEntity> fields, IRedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
        => db.HashSet((re ?? RedisEntity<TEntity>.Default).ReadKey(entity), fields.GetEntries(entity), flags);

    public static bool EntitySet<TEntity>(this IDatabase db, TEntity entity, RedisEntityField<TEntity> field, IRedisEntity<TEntity>? re = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSet((re ?? RedisEntity<TEntity>.Default).ReadKey(entity), field.RedisField, field.Read(entity), when, flags);

    public static bool EntityLoad<TEntity>(this IDatabase db, TEntity entity, RedisEntityField<TEntity> field, IRedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
        => field.Write(entity, db.HashGet((re ?? RedisEntity<TEntity>.Default).ReadKey(entity), field.RedisField, flags));

    public static bool EntityLoad<TEntity>(this IDatabase db, TEntity entity, RedisEntityFields<TEntity> fields, IRedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
        => fields.Write(entity, db.HashGet((re ?? RedisEntity<TEntity>.Default).ReadKey(entity), fields.RedisFields, flags));

    public static bool EntityLoad<TEntity>(this IDatabase db, TEntity entity, IRedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.Fields;
        return fields.Write(entity, db.HashGet(re.ReadKey(entity), fields.RedisFields, flags));
    }

    #endregion ReadKey

    public static void EntitySet<TEntity>(this IDatabase db, in RedisKey key, TEntity entity, RedisEntityFields<TEntity>? fields = null, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, (fields ?? RedisEntity<TEntity>.Default.Fields).ReadFields.GetEntries(entity), flags);

    public static bool EntitySet<TEntity>(this IDatabase db, in RedisKey key, TEntity entity, RedisEntityField<TEntity> field, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, field.RedisField, field.Read(entity), when, flags);

    public static bool EntitySetField<TEntity, TField>(this IDatabase db, in RedisKey key, RedisEntityField<TEntity> field, in TField value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, field.RedisField, field.GetFormatter<TField>().Serialize(in value), when, flags);

    public static bool EntityLoad<TEntity>(this IDatabase db, in RedisKey key, TEntity entity, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
        => field.Write(entity, db.HashGet(key, field.RedisField, flags));

    public static bool EntityLoad<TEntity>(this IDatabase db, in RedisKey key, TEntity entity, RedisEntityFields<TEntity>? fields = null, CommandFlags flags = CommandFlags.None)
    {
        fields ??= RedisEntity<TEntity>.Default.Fields;
        return fields.Write(entity, db.HashGet(key, fields.RedisFields, flags));
    }

    public static bool EntityLoadField<TEntity, TField>(this IDatabase db, in RedisKey key, ref TField? value, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
    {
        var redisValue = db.HashGet(key, field.RedisField, flags);

        if (redisValue.IsNull) return false;

        field.GetFormatter<TField>().Deserialize(in redisValue, ref value);

        return true;
    }

    public static TEntity? EntityGet<TEntity, IEntity>(this IDatabase db, in RedisKey key, RedisEntityField<IEntity> field, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
        => field.GetEntity<TEntity, IEntity>(db.HashGet(key, field.RedisField, flags));

    public static TEntity? EntityGet<TEntity, IEntity>(this IDatabase db, in RedisKey key, RedisEntityFields<IEntity>? fields = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
    {
        fields ??= RedisEntity<IEntity>.Default.Fields;
        return fields.GetEntity<TEntity, IEntity>(db.HashGet(key, fields.RedisFields, flags));
    }

    public static TEntity? EntityGet<TEntity>(this IDatabase db, in RedisKey key, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None) where TEntity : new()
        => field.GetEntity<TEntity, TEntity>(db.HashGet(key, field.RedisField, flags));

    public static TEntity? EntityGet<TEntity>(this IDatabase db, in RedisKey key, RedisEntityFields<TEntity>? fields = null, CommandFlags flags = CommandFlags.None) where TEntity : new()
    {
        fields ??= RedisEntity<TEntity>.Default.Fields;
        return fields.GetEntity<TEntity, TEntity>(db.HashGet(key, fields.RedisFields, flags));
    }

    public static TField? EntityGetField<TEntity, TField>(this IDatabase db, in RedisKey key, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
    {
        var redisValue = db.HashGet(key, field.RedisField, flags);

        TField? value = default;

        if (!redisValue.IsNull) field.GetFormatter<TField>().Deserialize(in redisValue, ref value);

        return value;
    }
}