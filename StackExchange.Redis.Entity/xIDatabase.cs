using StackExchange.Redis.Entity;

namespace StackExchange.Redis;

public static class xIDatabase
{
    public static void EntitySet<T>(this IDatabase db, RedisKey key, T entity, IRedisEntityReader<T>? reader = null, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, (reader ?? RedisEntity<T>.Reader).GetEntries(entity), flags);

    public static void EntitySet<T>(this IDatabase db, RedisKey key, T entity, RedisValue[] fields, IRedisEntityReader<T>? reader = null, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, (reader ?? RedisEntity<T>.Reader).GetEntries(entity, fields), flags);

    public static bool EntitySet<T>(this IDatabase db, RedisKey key, T entity, RedisValue field, IRedisEntityReader<T>? reader = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, field, (reader ?? RedisEntity<T>.Reader).Read(entity, in field), when, flags);

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

    public static T? EntityGetAll<T>(this IDatabase db, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
        => (writer ?? RedisEntity<T>.Writer).GetEntity(db.HashGetAll(key, flags));

    public static T? EntityGet<T>(this IDatabase db, RedisKey key, RedisValue field, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
        => (writer ?? RedisEntity<T>.Writer).GetEntity(in field, db.HashGet(key, field, flags));

    public static T? EntityGet<T>(this IDatabase db, RedisKey key, RedisValue[] fields, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
        => (writer ?? RedisEntity<T>.Writer).GetEntity(fields, db.HashGet(key, fields, flags));

    public static T? EntityGet<T>(this IDatabase db, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
    {
        writer ??= RedisEntity<T>.Writer;
        var fields = writer.Fields.All;
        return writer.GetEntity(fields, db.HashGet(key, fields, flags));
    }
}