using StackExchange.Redis.Entity;

namespace StackExchange.Redis;

public static class xIDatabase
{
    public static void EntitySet<T>(this IDatabase db, RedisKey key, T entity, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, RedisEntity<T>.GetEntries(entity), flags);

    public static void EntitySet<T>(this IDatabase db, RedisKey key, T entity, RedisValue[] fields, CommandFlags flags = CommandFlags.None)
        => db.HashSet(key, RedisEntity<T>.GetEntries(entity, fields), flags);

    public static void EntityGetAll<T>(this IDatabase db, RedisKey key, T entity, CommandFlags flags = CommandFlags.None)
        => RedisEntity<T>.GetEntity(db.HashGetAll(key, flags), entity);

    public static void EntityGet<T>(this IDatabase db, RedisKey key, T entity, RedisValue[] fields, CommandFlags flags = CommandFlags.None)
        => RedisEntity<T>.GetEntity(db.HashGet(key, fields, flags), fields, entity);

    public static void EntityGet<T>(this IDatabase db, RedisKey key, T entity, CommandFlags flags = CommandFlags.None)
        => RedisEntity<T>.GetEntity(db.HashGet(key, RedisEntity<T>.Fields, flags), RedisEntity<T>.Fields, entity);
}