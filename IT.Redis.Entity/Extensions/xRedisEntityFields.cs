namespace IT.Redis.Entity.Extensions;

public static class xRedisEntityFields
{
    public static void ReadEntries<TEntity>(this RedisEntityField<TEntity>[] fields, HashEntry[] entries, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (entries == null) throw new ArgumentNullException(nameof(entries));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        if (entries.Length < fields.Length + offset) throw new ArgumentOutOfRangeException(nameof(entries));

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            entries[offset++] = new HashEntry(field.ForRedis, field.Read(entity));
        }
    }

    public static void ReadFieldsAndValues<TEntity>(this RedisEntityField<TEntity>[] fields, RedisValue[] values, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (values == null) throw new ArgumentNullException(nameof(values));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        if (values.Length < fields.Length * 2 + offset) throw new ArgumentOutOfRangeException(nameof(values));

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            values[offset++] = field.ForRedis;
            values[offset++] = field.Read(entity);
        }
    }

    public static void ReadValues<TEntity>(this RedisEntityField<TEntity>[] fields, RedisValue[] values, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (values == null) throw new ArgumentNullException(nameof(values));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        if (values.Length < fields.Length + offset) throw new ArgumentOutOfRangeException(nameof(values));

        for (int i = 0; i < fields.Length; i++)
        {
            values[offset++] = fields[i].Read(entity);
        }
    }

    public static void ReadOddValues<TEntity>(this RedisEntityField<TEntity>[] fields, RedisValue[] values, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (values == null) throw new ArgumentNullException(nameof(values));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        if (values.Length < fields.Length * 2 + offset) throw new ArgumentOutOfRangeException(nameof(values));
        offset++;
        for (int i = 0; i < fields.Length; i++, offset += 2)
        {
            values[offset] = fields[i].Read(entity);
        }
    }

    public static HashEntry[] GetEntries<TEntity>(this RedisEntityField<TEntity>[] fields, TEntity entity, int offset = 0)
    {
        var entries = new HashEntry[fields.Length + offset];

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            entries[offset++] = new HashEntry(field.ForRedis, field.Read(entity));
        }

        return entries;
    }

    public static RedisValue[] GetFieldsAndValues<TEntity>(this RedisEntityField<TEntity>[] fields, TEntity entity, int offset = 0)
    {
        var values = new RedisValue[fields.Length * 2 + offset];

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            values[offset++] = field.ForRedis;
            values[offset++] = field.Read(entity);
        }

        return values;
    }

    public static RedisValue[] GetValues<TEntity>(this RedisEntityField<TEntity>[] fields, TEntity entity, int offset = 0)
    {
        var values = new RedisValue[fields.Length + offset];

        for (int i = 0; i < fields.Length; i++)
        {
            values[offset++] = fields[i].Read(entity);
        }

        return values;
    }

    public static RedisValue[] GetEvenFields<TEntity>(this RedisEntityField<TEntity>[] fields, int offset = 0)
    {
        var values = new RedisValue[fields.Length * 2 + offset];

        for (int i = 0; i < fields.Length; i++, offset += 2)
        {
            values[offset] = fields[i].ForRedis;
        }

        return values;
    }

    public static bool Write<TEntity>(this RedisEntityField<TEntity>[] fields, TEntity entity, RedisValue[] values, int offset = 0)
    {
        if (values.Length < fields.Length + offset) throw new ArgumentOutOfRangeException(nameof(values));

        var writen = false;
        for (int i = 0; i < fields.Length; i++)
        {
            var value = values[offset++];
            if (!value.IsNull)
            {
                fields[i].Write(entity, in value);
                writen = true;
            }
        }
        return writen;
    }

    public static TEntity? GetEntity<TEntity, IEntity>(this RedisEntityField<IEntity>[] fields, RedisValue[] values, Func<TEntity> newEntity, int offset = 0) where TEntity : IEntity
    {
        if (values.Length < fields.Length + offset) throw new ArgumentOutOfRangeException(nameof(values));

        int i = 0;
        RedisValue value;

        for (; i < fields.Length; i++)
        {
            value = values[offset++];
            if (!value.IsNull) goto write;
        }
        return default;
    write:
        var entity = newEntity();

        fields[i++].Write(entity, in value);

        for (; i < fields.Length; i++)
        {
            value = values[offset++];
            if (!value.IsNull) fields[i].Write(entity, in value);
        }
        return entity;
    }

    public static TEntity? GetEntity<TEntity>(this RedisEntityField<TEntity>[] fields, RedisValue[] values, Func<TEntity> newEntity, int offset = 0)
        => fields.GetEntity<TEntity, TEntity>(values, newEntity, offset);

    public static TEntity? GetEntity<TEntity, IEntity>(this RedisEntityField<IEntity>[] fields, RedisValue[] values, int offset = 0) where TEntity : IEntity, new()
    {
        if (values.Length < fields.Length + offset) throw new ArgumentOutOfRangeException(nameof(values));

        int i = 0;
        RedisValue value;

        for (; i < fields.Length; i++)
        {
            value = values[offset++];
            if (!value.IsNull) goto write;
        }
        return default;
    write:
        var entity = new TEntity();

        fields[i++].Write(entity, in value);

        for (; i < fields.Length; i++)
        {
            value = values[offset++];
            if (!value.IsNull) fields[i].Write(entity, in value);
        }
        return entity;
    }

    public static TEntity? GetEntity<TEntity>(this RedisEntityField<TEntity>[] fields, RedisValue[] values, int offset = 0) where TEntity : new()
        => fields.GetEntity<TEntity, TEntity>(values, offset);
}