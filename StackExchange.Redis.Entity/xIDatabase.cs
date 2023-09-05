using StackExchange.Redis.Entity;

namespace StackExchange.Redis;

public static class xIDatabase
{
    public static void EntitySet<T>(this IDatabase db, RedisKey key, T entity, IRedisEntityReader<T>? reader = null, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, (reader ?? RedisEntity<T>.Reader).ReadAllFields(entity), flags);

    public static void EntitySet<T>(this IDatabase db, RedisKey key, T entity, RedisValue[] fields, IRedisEntityReader<T>? reader = null, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, (reader ?? RedisEntity<T>.Reader).ReadFields(entity, fields), flags);

    public static void EntityLoadAll<T>(this IDatabase db, T entity, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
        => (writer ?? RedisEntity<T>.Writer).Write(entity, db.HashGetAll(key, flags));

    public static void EntityLoad<T>(this IDatabase db, T entity, RedisKey key, RedisValue[] fields, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
        => (writer ?? RedisEntity<T>.Writer).Write(entity, fields, db.HashGet(key, fields, flags));

    public static void EntityLoad<T>(this IDatabase db, T entity, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
    {
        writer ??= RedisEntity<T>.Writer;
        writer.Write(entity, writer.Fields.All, db.HashGet(key, writer.Fields.All, flags));
    }

    public static T? EntityGetAll<T>(this IDatabase db, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
    {
        var entries = db.HashGetAll(key, flags);

        if (entries.Length == 0) return default;

        var entity = new T();

        (writer ?? RedisEntity<T>.Writer).Write(entity, entries);

        return entity;
    }

    public static T? EntityGet<T>(this IDatabase db, RedisKey key, RedisValue[] fields, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
    {
        var values = db.HashGet(key, fields, flags);

        int i = 0;
        RedisValue value;

        for (; i < values.Length; i++)
        {
            value = values[i];
            if (!value.IsNull) goto write;
        }
        return default;
    write:
        writer ??= RedisEntity<T>.Writer;

        var entity = new T();

        writer.Write(entity, in fields[i++], in value);

        for (; i < values.Length; i++)
        {
            value = values[i];
            if (!value.IsNull) writer.Write(entity, in fields[i], in value);
        }
        return entity;
    }

    public static T? EntityGet<T>(this IDatabase db, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
    {
        writer ??= RedisEntity<T>.Writer;
        return db.EntityGet(key, writer.Fields.All, writer, flags);
    }
}