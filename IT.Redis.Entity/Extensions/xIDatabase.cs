namespace IT.Redis.Entity.Extensions;

public static class xIDatabase
{
    #region ReadKey

    public static void EntitySet<TEntity>(this IDatabase db, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        db.HashSet(re.ReadKey(entity), re.ReadFields.Array.GetEntries(entity), flags);
    }

    public static bool EntitySet<TEntity>(this IDatabase db, TEntity entity, RedisEntityField<TEntity> field, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSet(field.ReadKey(entity), field.RedisValue, field.Read(entity), when, flags);

    public static bool EntityLoad<TEntity>(this IDatabase db, TEntity entity, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
        => field.Write(entity, db.HashGet(field.ReadKey(entity), field.RedisValue, flags));

    public static bool EntityLoad<TEntity>(this IDatabase db, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.WriteFields;
        return fields.Array.Write(entity, db.HashGet(re.ReadKey(entity), fields.RedisValues, flags));
    }

    #endregion ReadKey

    public static void EntitySet<TEntity>(this IDatabase db, in RedisKey key, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, (re ?? RedisEntity<TEntity>.Default).ReadFields.Array.GetEntries(entity), flags);

    public static bool EntitySet<TEntity>(this IDatabase db, in RedisKey key, TEntity entity, RedisEntityField<TEntity> field, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, field.RedisValue, field.Read(entity), when, flags);

    public static bool EntitySetField<TEntity, TField>(this IDatabase db, in RedisKey key, RedisEntityField<TEntity> field, in TField value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, field.RedisValue, field.GetFormatter<TField>().Serialize(in value), when, flags);

    public static bool EntityLoad<TEntity>(this IDatabase db, in RedisKey key, TEntity entity, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
        => field.Write(entity, db.HashGet(key, field.RedisValue, flags));

    public static bool EntityLoad<TEntity>(this IDatabase db, in RedisKey key, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.WriteFields;
        return fields.Array.Write(entity, db.HashGet(key, fields.RedisValues, flags));
    }

    public static bool EntityLoadField<TEntity, TField>(this IDatabase db, in RedisKey key, ref TField? value, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
    {
        var redisValue = db.HashGet(key, field.RedisValue, flags);

        if (redisValue.IsNull) return false;

        field.GetFormatter<TField>().Deserialize(in redisValue, ref value);

        return true;
    }

    public static TEntity? EntityGet<TEntity, IEntity>(this IDatabase db, in RedisKey key, RedisEntityField<IEntity> field, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
        => field.GetEntity<TEntity, IEntity>(db.HashGet(key, field.RedisValue, flags));

    public static TEntity? EntityGet<TEntity, IEntity>(this IDatabase db, in RedisKey key, RedisEntity<IEntity>? re = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
    {
        re ??= RedisEntity<IEntity>.Default;
        var fields = re.WriteFields;
        return fields.Array.GetEntity<TEntity, IEntity>(db.HashGet(key, fields.RedisValues, flags));
    }

    public static TEntity? EntityGet<TEntity>(this IDatabase db, in RedisKey key, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None) where TEntity : new()
        => field.GetEntity<TEntity, TEntity>(db.HashGet(key, field.RedisValue, flags));

    public static TEntity? EntityGet<TEntity>(this IDatabase db, in RedisKey key, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None) where TEntity : new()
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.WriteFields;
        return fields.Array.GetEntity<TEntity, TEntity>(db.HashGet(key, fields.RedisValues, flags));
    }

    public static TField? EntityGetField<TEntity, TField>(this IDatabase db, in RedisKey key, RedisEntityField<TEntity> field, TField? defaultValue = default, CommandFlags flags = CommandFlags.None)
    {
        var redisValue = db.HashGet(key, field.RedisValue, flags);
        if (redisValue.IsNull) return defaultValue;

        TField? value = default;
        field.GetFormatter<TField>().Deserialize(in redisValue, ref value);
        return value;
    }

    public static TField? EntityGetField<TEntity, TField>(this IDatabase db, in RedisKey key, RedisEntityField<TEntity> field, Func<TField?> getDefaultValue, CommandFlags flags = CommandFlags.None)
    {
        var redisValue = db.HashGet(key, field.RedisValue, flags);
        if (redisValue.IsNull) return getDefaultValue();

        TField? value = default;
        field.GetFormatter<TField>().Deserialize(in redisValue, ref value);
        return value;
    }
}