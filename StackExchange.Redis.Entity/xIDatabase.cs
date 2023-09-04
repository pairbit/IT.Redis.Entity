using StackExchange.Redis.Entity;

namespace StackExchange.Redis;

public static class xIDatabase
{
    public static void EntitySet<T>(this IDatabase db, RedisKey key, T entity, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, RedisEntity<T>.GetEntries(entity), flags);

    public static void EntitySet<T>(this IDatabase db, RedisKey key, T entity, RedisValue[] fields, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, RedisEntity<T>.GetEntries(entity, fields), flags);

    public static void EntityLoadAll<T>(this IDatabase db, T entity, RedisKey key, CommandFlags flags = CommandFlags.None)
        => RedisEntity<T>.LoadEntity(entity, db.HashGetAll(key, flags));

    public static void EntityLoad<T>(this IDatabase db, T entity, RedisKey key, RedisValue[] fields, CommandFlags flags = CommandFlags.None)
        => RedisEntity<T>.LoadEntity(entity, db.HashGet(key, fields, flags), fields);

    public static void EntityLoad<T>(this IDatabase db, T entity, RedisKey key, CommandFlags flags = CommandFlags.None)
        => RedisEntity<T>.LoadEntity(entity, db.HashGet(key, RedisEntity<T>.Fields, flags), RedisEntity<T>.Fields);

    public static T EntityGetAll<T>(this IDatabase db, RedisKey key, CommandFlags flags = CommandFlags.None) where T : new()
    {
        var entity = new T();
        RedisEntity<T>.LoadEntity(entity, db.HashGetAll(key, flags));
        return entity;
    }

    public static T EntityGet<T>(this IDatabase db, RedisKey key, RedisValue[] fields, CommandFlags flags = CommandFlags.None) where T : new()
    {
        var entity = new T();
        RedisEntity<T>.LoadEntity(entity, db.HashGet(key, fields, flags), fields);
        return entity;
    }

    public static T EntityGet<T>(this IDatabase db, RedisKey key, CommandFlags flags = CommandFlags.None) where T : new()
    {
        var entity = new T();
        RedisEntity<T>.LoadEntity(entity, db.HashGet(key, RedisEntity<T>.Fields, flags), RedisEntity<T>.Fields);
        return entity;
    }
}