namespace IT.Redis.Entity.Extensions;

public static class xRedisEntityFields
{
    public static void ReadEntries<TEntity>(this RedisEntityFields<TEntity> fields, HashEntry[] entries, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (entries == null) throw new ArgumentNullException(nameof(entries));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        var array = fields.Array;
        if (entries.Length < array.Length + offset) throw new ArgumentOutOfRangeException(nameof(entries));

        for (int i = 0; i < array.Length; i++)
        {
            var field = array[i];
            entries[i + offset] = new HashEntry(field.ForRedis, field.Read(entity));
        }
    }

    public static void ReadFieldsAndValues<TEntity>(this RedisEntityFields<TEntity> fields, RedisValue[] values, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (values == null) throw new ArgumentNullException(nameof(values));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        var array = fields.Array;
        if (values.Length < array.Length * 2 + offset) throw new ArgumentOutOfRangeException(nameof(values));

        for (int i = 0; i < array.Length; i++)
        {
            var field = array[i];
            values[offset++] = field.ForRedis;
            values[offset++] = field.Read(entity);
        }
    }

    public static void ReadValues<TEntity>(this RedisEntityFields<TEntity> fields, RedisValue[] values, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (values == null) throw new ArgumentNullException(nameof(values));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        var array = fields.Array;
        if (values.Length < array.Length + offset) throw new ArgumentOutOfRangeException(nameof(values));

        for (int i = 0; i < array.Length; i++)
        {
            values[offset++] = array[i].Read(entity);
        }
    }

    public static void ReadOddValues<TEntity>(this RedisEntityFields<TEntity> fields, RedisValue[] values, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (values == null) throw new ArgumentNullException(nameof(values));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        var array = fields.Array;
        if (values.Length < array.Length * 2 + offset) throw new ArgumentOutOfRangeException(nameof(values));
        offset++;
        for (int i = 0; i < array.Length; i++)
        {
            values[offset += 2] = array[i].Read(entity);
        }
    }

    public static HashEntry[] GetEntries<TEntity>(this RedisEntityFields<TEntity> fields, TEntity entity)
    {
        var array = fields.Array;

        var entries = new HashEntry[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            var field = array[i];
            entries[i] = new HashEntry(field.ForRedis, field.Read(entity));
        }

        return entries;
    }

    public static RedisValue[] GetFieldsAndValues<TEntity>(this RedisEntityFields<TEntity> fields, TEntity entity, int offset = 0)
    {
        var array = fields.Array;

        var values = new RedisValue[array.Length * 2 + offset];

        for (int i = 0; i < array.Length; i++)
        {
            var field = array[i];
            values[offset++] = field.ForRedis;
            values[offset++] = field.Read(entity);
        }

        return values;
    }

    public static RedisValue[] GetValues<TEntity>(this RedisEntityFields<TEntity> fields, TEntity entity, int offset = 0)
    {
        var array = fields.Array;

        var values = new RedisValue[array.Length + offset];

        for (int i = 0; i < array.Length; i++)
        {
            values[offset++] = array[i].Read(entity);
        }

        return values;
    }

    public static RedisValue[] GetEvenFields<TEntity>(this RedisEntityFields<TEntity> fields, int offset = 0)
    {
        var array = fields.Array;

        var values = new RedisValue[array.Length * 2 + offset];

        for (int i = 0; i < array.Length; i++)
        {
            values[offset += 2] = array[i].ForRedis;
        }

        return values;
    }

    public static bool Write<TEntity>(this RedisEntityFields<TEntity> fields, TEntity entity, RedisValue[] values)
    {
        var array = fields.Array;
        if (array.Length != values.Length) throw new ArgumentOutOfRangeException(nameof(values));

        var writen = false;
        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (!value.IsNull)
            {
                array[i].Write(entity, in value);
                writen = true;
            }
        }
        return writen;
    }

    public static TEntity? GetEntity<TEntity, IEntity>(this RedisEntityFields<IEntity> fields, RedisValue[] values, Func<TEntity> newEntity) where TEntity : IEntity
    {
        int i = 0;
        RedisValue value;

        for (; i < values.Length; i++)
        {
            value = values[i];
            if (!value.IsNull) goto write;
        }
        return default;
    write:
        var entity = newEntity();

        var array = fields.Array;

        array[i++].Write(entity, in value);

        for (; i < values.Length; i++)
        {
            value = values[i];
            if (!value.IsNull) array[i].Write(entity, in value);
        }
        return entity;
    }

    public static TEntity? GetEntity<TEntity>(this RedisEntityFields<TEntity> fields, RedisValue[] values, Func<TEntity> newEntity)
        => fields.GetEntity<TEntity, TEntity>(values, newEntity);

    public static TEntity? GetEntity<TEntity, IEntity>(this RedisEntityFields<IEntity> fields, RedisValue[] values) where TEntity : IEntity, new()
    {
        int i = 0;
        RedisValue value;

        for (; i < values.Length; i++)
        {
            value = values[i];
            if (!value.IsNull) goto write;
        }
        return default;
    write:
        var entity = new TEntity();

        var array = fields.Array;

        array[i++].Write(entity, in value);

        for (; i < values.Length; i++)
        {
            value = values[i];
            if (!value.IsNull) array[i].Write(entity, in value);
        }
        return entity;
    }

    public static TEntity? GetEntity<TEntity>(this RedisEntityFields<TEntity> fields, RedisValue[] values) where TEntity : new()
        => fields.GetEntity<TEntity, TEntity>(values);
}