namespace IT.Redis.Entity.Extensions;

public static class xIDatabaseAsync
{
    #region ReadKey

    public static Task EntitySetAsync<TEntity>(this IDatabaseAsync db, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        return db.HashSetAsync(re.ReadKey(entity), re.ReadFields.GetEntries(entity), flags);
    }

    public static Task<bool> EntitySetAsync<TEntity>(this IDatabaseAsync db, TEntity entity, RedisEntityField<TEntity> field, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSetAsync(field.ReadKey(entity), field.ForRedis, field.Read(entity), when, flags);

    public static async Task<bool> EntityLoadAsync<TEntity>(this IDatabaseAsync db, TEntity entity, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
        => field.Write(entity, await db.HashGetAsync(field.ReadKey(entity), field.ForRedis, flags).ConfigureAwait(false));

    public static async Task<bool> EntityLoadAsync<TEntity>(this IDatabaseAsync db, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.WriteFields;
        return fields.Write(entity, await db.HashGetAsync(re.ReadKey(entity), fields.ForRedis, flags).ConfigureAwait(false));
    }

    #endregion ReadKey

    public static Task EntitySetAsync<TEntity>(this IDatabaseAsync db, in RedisKey key, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
        => db.HashSetAsync(key, (re ?? RedisEntity<TEntity>.Default).ReadFields.GetEntries(entity), flags);

    public static Task<bool> EntitySetAsync<TEntity>(this IDatabaseAsync db, in RedisKey key, TEntity entity, RedisEntityField<TEntity> field, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSetAsync(key, field.ForRedis, field.Read(entity), when, flags);

    public static Task<bool> EntitySetFieldAsync<TEntity, TField>(this IDatabaseAsync db, in RedisKey key, RedisEntityField<TEntity> field, in TField value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSetAsync(key, field.ForRedis, field.GetFormatter<TField>().Serialize(in value), when, flags);

    public static async Task<bool> EntityLoadAsync<TEntity>(this IDatabaseAsync db, RedisKey key, TEntity entity, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
        => field.Write(entity, await db.HashGetAsync(key, field.ForRedis, flags).ConfigureAwait(false));

    public static async Task<bool> EntityLoadAsync<TEntity>(this IDatabaseAsync db, RedisKey key, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.WriteFields;
        return fields.Write(entity, await db.HashGetAsync(key, fields.ForRedis, flags).ConfigureAwait(false));
    }

    /*public static bool EntityLoadField<TEntity, TField>(this IDatabase db, in RedisKey key, ref TField? value, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
    {
        var redisValue = db.HashGet(key, field.ForRedis, flags);

        if (redisValue.IsNull) return false;

        field.GetFormatter<TField>().Deserialize(in redisValue, ref value);

        return true;
    }*/

    public static async Task<TEntity?> EntityGetAsync<TEntity, IEntity>(this IDatabaseAsync db, RedisKey key, RedisEntityField<IEntity> field, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
        => field.GetEntity<TEntity, IEntity>(await db.HashGetAsync(key, field.ForRedis, flags).ConfigureAwait(false));

    public static async Task<TEntity?> EntityGetAsync<TEntity, IEntity>(this IDatabaseAsync db, RedisKey key, RedisEntity<IEntity>? re = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
    {
        re ??= RedisEntity<IEntity>.Default;
        var fields = re.WriteFields;
        return fields.GetEntity<TEntity, IEntity>(await db.HashGetAsync(key, fields.ForRedis, flags).ConfigureAwait(false));
    }

    public static async Task<TEntity?> EntityGetAsync<TEntity>(this IDatabaseAsync db, RedisKey key, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None) where TEntity : new()
        => field.GetEntity<TEntity, TEntity>(await db.HashGetAsync(key, field.ForRedis, flags).ConfigureAwait(false));

    public static async Task<TEntity?> EntityGetAsync<TEntity>(this IDatabaseAsync db, RedisKey key, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None) where TEntity : new()
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.WriteFields;
        return fields.GetEntity<TEntity, TEntity>(await db.HashGetAsync(key, fields.ForRedis, flags).ConfigureAwait(false));
    }

    public static async Task<TField?> EntityGetFieldAsync<TEntity, TField>(this IDatabaseAsync db, RedisKey key, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
    {
        var redisValue = await db.HashGetAsync(key, field.ForRedis, flags).ConfigureAwait(false);
        if (redisValue.IsNull) return default;

        TField? value = default;
        field.GetFormatter<TField>().Deserialize(in redisValue, ref value);
        return value;
    }
}