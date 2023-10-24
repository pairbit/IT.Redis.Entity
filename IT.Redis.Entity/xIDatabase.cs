using IT.Redis.Entity;

namespace StackExchange.Redis;

public static class xIDatabase
{
    public static void EntitySet<T>(this IDatabase db, T entity, IRedisEntityReader<T>? reader = null, CommandFlags flags = CommandFlags.None)
    {
        reader ??= RedisEntity<T>.Reader;
        db.HashSet(reader.ReadKey(entity), reader.GetEntries(entity), flags);
    }

    public static void EntitySet<T>(this IDatabase db, T entity, RedisValue[] fields, IRedisEntityReader<T>? reader = null, CommandFlags flags = CommandFlags.None)
    {
        reader ??= RedisEntity<T>.Reader;
        db.HashSet(reader.ReadKey(entity), reader.GetEntries(entity, fields), flags);
    }

    public static bool EntitySet<T>(this IDatabase db, T entity, RedisValue field, IRedisEntityReader<T>? reader = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        reader ??= RedisEntity<T>.Reader;
        return db.HashSet(reader.ReadKey(entity), field, reader.Read(entity, in field), when, flags);
    }

    public static bool EntityLoadAll<T>(this IDatabase db, T entity, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
    {
        writer ??= RedisEntity<T>.Writer;
        return writer.Write(entity, db.HashGetAll(writer.ReadKey(entity), flags));
    }

    public static bool EntityLoad<T>(this IDatabase db, T entity, RedisValue field, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
    {
        writer ??= RedisEntity<T>.Writer;
        return writer.Write(entity, in field, db.HashGet(writer.ReadKey(entity), field, flags));
    }

    public static bool EntityLoad<T>(this IDatabase db, T entity, RedisValue[] fields, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
    {
        writer ??= RedisEntity<T>.Writer;
        return writer.Write(entity, fields, db.HashGet(writer.ReadKey(entity), fields, flags));
    }

    public static bool EntityLoad<T>(this IDatabase db, T entity, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
    {
        writer ??= RedisEntity<T>.Writer;
        var fields = writer.Fields.All;
        return writer.Write(entity, fields, db.HashGet(writer.ReadKey(entity), fields, flags));
    }

    public static void EntitySet<T>(this IDatabase db, RedisKey key, T entity, IRedisEntityReader<T>? reader = null, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, (reader ?? RedisEntity<T>.Reader).GetEntries(entity), flags);

    public static void EntitySet<T>(this IDatabase db, RedisKey key, T entity, RedisValue[] fields, IRedisEntityReader<T>? reader = null, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, (reader ?? RedisEntity<T>.Reader).GetEntries(entity, fields), flags);

    public static bool EntitySet<T>(this IDatabase db, RedisKey key, T entity, RedisValue field, IRedisEntityReader<T>? reader = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, field, (reader ?? RedisEntity<T>.Reader).Read(entity, in field), when, flags);

    public static bool EntitySetField<TEntity, TField>(this IDatabase db, in RedisKey key, in RedisValue field, in TField value, IRedisEntityReader<TEntity>? reader = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, field, (reader ?? RedisEntity<TEntity>.Reader).GetSerializer<TField>(in field).Serialize(in value), when, flags);

    public static bool EntityLoadAll<T>(this IDatabase db, T entity, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
        => (writer ?? RedisEntity<T>.Writer).Write(entity, db.HashGetAll(key, flags));

    public static bool EntityLoad<T>(this IDatabase db, T entity, RedisKey key, RedisValue field, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
        => (writer ?? RedisEntity<T>.Writer).Write(entity, in field, db.HashGet(key, field, flags));

    public static bool EntityLoad<T>(this IDatabase db, T entity, RedisKey key, RedisValue[] fields, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
        => (writer ?? RedisEntity<T>.Writer).Write(entity, fields, db.HashGet(key, fields, flags));

    public static bool EntityLoad<T>(this IDatabase db, T entity, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
    {
        writer ??= RedisEntity<T>.Writer;
        var fields = writer.Fields.All;
        return writer.Write(entity, fields, db.HashGet(key, fields, flags));
    }

    public static bool EntityLoadField<TEntity, TField>(this IDatabase db, ref TField? value, in RedisKey key, in RedisValue field, IRedisEntityWriter<TEntity>? writer = null, CommandFlags flags = CommandFlags.None)
    {
        var redisValue = db.HashGet(key, field, flags);

        if (redisValue.IsNull) return false;

        (writer ?? RedisEntity<TEntity>.Writer).GetDeserializer<TField>(in field).Deserialize(redisValue, ref value);

        return true;
    }

    public static TEntity? EntityGetAll<TEntity, IEntity>(this IDatabase db, RedisKey key, IRedisEntityWriter<IEntity>? writer = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
        => (writer ?? RedisEntity<IEntity>.Writer).GetEntity<TEntity, IEntity>(db.HashGetAll(key, flags));

    public static TEntity? EntityGet<TEntity, IEntity>(this IDatabase db, RedisKey key, RedisValue field, IRedisEntityWriter<IEntity>? writer = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
        => (writer ?? RedisEntity<IEntity>.Writer).GetEntity<TEntity, IEntity>(in field, db.HashGet(key, field, flags));

    public static TEntity? EntityGet<TEntity, IEntity>(this IDatabase db, RedisKey key, RedisValue[] fields, IRedisEntityWriter<IEntity>? writer = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
        => (writer ?? RedisEntity<IEntity>.Writer).GetEntity<TEntity, IEntity>(fields, db.HashGet(key, fields, flags));

    public static TEntity? EntityGet<TEntity, IEntity>(this IDatabase db, RedisKey key, IRedisEntityWriter<IEntity>? writer = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
    {
        writer ??= RedisEntity<IEntity>.Writer;
        var fields = writer.Fields.All;
        return writer.GetEntity<TEntity, IEntity>(fields, db.HashGet(key, fields, flags));
    }

    public static T? EntityGetAll<T>(this IDatabase db, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
        => (writer ?? RedisEntity<T>.Writer).GetEntity<T, T>(db.HashGetAll(key, flags));

    public static T? EntityGet<T>(this IDatabase db, RedisKey key, RedisValue field, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
        => (writer ?? RedisEntity<T>.Writer).GetEntity<T, T>(in field, db.HashGet(key, field, flags));

    public static T? EntityGet<T>(this IDatabase db, RedisKey key, RedisValue[] fields, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
        => (writer ?? RedisEntity<T>.Writer).GetEntity<T, T>(fields, db.HashGet(key, fields, flags));

    public static T? EntityGet<T>(this IDatabase db, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
    {
        writer ??= RedisEntity<T>.Writer;
        var fields = writer.Fields.All;
        return writer.GetEntity<T, T>(fields, db.HashGet(key, fields, flags));
    }

    public static TField? EntityGetField<TEntity, TField>(this IDatabase db, in RedisKey key, in RedisValue field, IRedisEntityWriter<TEntity>? writer = null, CommandFlags flags = CommandFlags.None)
    {
        TField? value = default;

        var redisValue = db.HashGet(key, field, flags);

        if (!redisValue.IsNull) (writer ?? RedisEntity<TEntity>.Writer).GetDeserializer<TField>(in field).Deserialize(redisValue, ref value);

        return value;
    }
}