using StackExchange.Redis.Entity;

namespace StackExchange.Redis;

public static class xIDatabase
{
    public static void EntitySet<T>(this IDatabase db, RedisKey key, T entity, IRedisEntityReader<T>? reader = null, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, (reader ?? RedisEntity<T>.Reader).ReadAllFields(entity), flags);

    public static void EntitySet<T>(this IDatabase db, RedisKey key, T entity, RedisValue[] fields, IRedisEntityReader<T>? reader = null, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, (reader ?? RedisEntity<T>.Reader).ReadFields(entity, fields), flags);

    public static bool EntityLoadAll<T>(this IDatabase db, T entity, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
        => (writer ?? RedisEntity<T>.Writer).Write(entity, db.HashGetAll(key, flags));

    public static bool EntityLoad<T>(this IDatabase db, T entity, RedisKey key, RedisValue[] fields, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
        => (writer ?? RedisEntity<T>.Writer).Write(entity, fields, db.HashGet(key, fields, flags));

    public static bool EntityLoad<T>(this IDatabase db, T entity, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None)
    {
        writer ??= RedisEntity<T>.Writer;
        return writer.Write(entity, writer.Fields.All, db.HashGet(key, writer.Fields.All, flags));
    }

    public static T? EntityGetAll<T>(this IDatabase db, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
        => (writer ?? RedisEntity<T>.Writer).Get(db.HashGetAll(key, flags));

    public static T? EntityGet<T>(this IDatabase db, RedisKey key, RedisValue[] fields, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
        => (writer ?? RedisEntity<T>.Writer).Get(fields, db.HashGet(key, fields, flags));

    public static T? EntityGet<T>(this IDatabase db, RedisKey key, IRedisEntityWriter<T>? writer = null, CommandFlags flags = CommandFlags.None) where T : new()
    {
        writer ??= RedisEntity<T>.Writer;
        var fields = writer.Fields.All;
        return writer.Get(fields, db.HashGet(key, fields, flags));
    }
}