namespace IT.Redis.Entity.Extensions;

public static class xIDatabaseAsync
{
    #region ReadKey

    public static Task EntitySetAsync<TEntity>(this IDatabaseAsync db, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        return db.HashSetAsync(re.ReadKey(entity), re.ReadFields.Array.GetEntries(entity), flags);
    }

    public static Task<bool> EntitySetAsync<TEntity>(this IDatabaseAsync db, TEntity entity, RedisEntityField<TEntity> field, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSetAsync(field.ReadKey(entity), field.RedisValue, field.Read(entity), when, flags);

    public static async Task<bool> EntityLoadAsync<TEntity>(this IDatabaseAsync db, TEntity entity, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
        => field.Write(entity, await db.HashGetAsync(field.ReadKey(entity), field.RedisValue, flags).ConfigureAwait(false));

    public static async Task<bool> EntityLoadAsync<TEntity>(this IDatabaseAsync db, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.WriteFields;
        return fields.Array.Write(entity, await db.HashGetAsync(re.ReadKey(entity), fields.RedisValues, flags).ConfigureAwait(false));
    }

    #endregion ReadKey

    public static Task EntitySetAsync<TEntity>(this IDatabaseAsync db, in RedisKey key, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
        => db.HashSetAsync(key, (re ?? RedisEntity<TEntity>.Default).ReadFields.Array.GetEntries(entity), flags);

    public static Task<bool> EntitySetAsync<TEntity>(this IDatabaseAsync db, in RedisKey key, TEntity entity, RedisEntityField<TEntity> field, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSetAsync(key, field.RedisValue, field.Read(entity), when, flags);

    public static Task<bool> EntitySetFieldAsync<TEntity, TField>(this IDatabaseAsync db, in RedisKey key, RedisEntityField<TEntity> field, in TField value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        => db.HashSetAsync(key, field.RedisValue, field.GetFormatter<TField>().Serialize(in value), when, flags);

    public static async Task<bool> EntityLoadAsync<TEntity>(this IDatabaseAsync db, RedisKey key, TEntity entity, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
        => field.Write(entity, await db.HashGetAsync(key, field.RedisValue, flags).ConfigureAwait(false));

    public static async Task<bool> EntityLoadAsync<TEntity>(this IDatabaseAsync db, RedisKey key, TEntity entity, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None)
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.WriteFields;
        return fields.Array.Write(entity, await db.HashGetAsync(key, fields.RedisValues, flags).ConfigureAwait(false));
    }

    /*public static bool EntityLoadField<TEntity, TField>(this IDatabase db, in RedisKey key, ref TField? value, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None)
    {
        var redisValue = db.HashGet(key, field.RedisValue, flags);

        if (redisValue.IsNull) return false;

        field.GetFormatter<TField>().Deserialize(in redisValue, ref value);

        return true;
    }*/

    public static async Task<TEntity?> EntityGetAsync<TEntity, IEntity>(this IDatabaseAsync db, RedisKey key, RedisEntityField<IEntity> field, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
        => field.GetEntity<TEntity, IEntity>(await db.HashGetAsync(key, field.RedisValue, flags).ConfigureAwait(false));

    public static async Task<TEntity?> EntityGetAsync<TEntity, IEntity>(this IDatabaseAsync db, RedisKey key, RedisEntity<IEntity>? re = null, CommandFlags flags = CommandFlags.None) where TEntity : IEntity, new()
    {
        re ??= RedisEntity<IEntity>.Default;
        var fields = re.WriteFields;
        return fields.Array.GetEntity<TEntity, IEntity>(await db.HashGetAsync(key, fields.RedisValues, flags).ConfigureAwait(false));
    }

    public static async Task<TEntity?> EntityGetAsync<TEntity>(this IDatabaseAsync db, RedisKey key, RedisEntityField<TEntity> field, CommandFlags flags = CommandFlags.None) where TEntity : new()
        => field.GetEntity<TEntity, TEntity>(await db.HashGetAsync(key, field.RedisValue, flags).ConfigureAwait(false));

    public static async Task<TEntity?> EntityGetAsync<TEntity>(this IDatabaseAsync db, RedisKey key, RedisEntity<TEntity>? re = null, CommandFlags flags = CommandFlags.None) where TEntity : new()
    {
        re ??= RedisEntity<TEntity>.Default;
        var fields = re.WriteFields;
        return fields.Array.GetEntity<TEntity, TEntity>(await db.HashGetAsync(key, fields.RedisValues, flags).ConfigureAwait(false));
    }

    public static async Task<TField?> EntityGetFieldAsync<TEntity, TField>(this IDatabaseAsync db, RedisKey key, RedisEntityField<TEntity> field, TField? defaultValue = default, CommandFlags flags = CommandFlags.None)
    {
        var redisValue = await db.HashGetAsync(key, field.RedisValue, flags).ConfigureAwait(false);
        if (redisValue.IsNull) return defaultValue;

        TField? value = default;
        field.GetFormatter<TField>().Deserialize(in redisValue, ref value);
        return value;
    }

    public static async Task<TField?> EntityGetFieldAsync<TEntity, TField>(this IDatabaseAsync db, RedisKey key, RedisEntityField<TEntity> field, Func<TField?> getDefaultValue, CommandFlags flags = CommandFlags.None)
    {
        var redisValue = await db.HashGetAsync(key, field.RedisValue, flags).ConfigureAwait(false);
        if (redisValue.IsNull) return getDefaultValue();

        TField? value = default;
        field.GetFormatter<TField>().Deserialize(in redisValue, ref value);
        return value;
    }
}