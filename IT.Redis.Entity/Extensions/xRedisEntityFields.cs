namespace IT.Redis.Entity.Extensions;

public static class xRedisEntityFields
{
    public static void ReadEntries<TEntity>(this RedisEntityFields<TEntity> fields, HashEntry[] entries, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (entries == null) throw new ArgumentNullException(nameof(entries));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        var entityFields = fields.EntityFields;
        if (entries.Length < entityFields.Length + offset) throw new ArgumentOutOfRangeException(nameof(entries));

        for (int i = 0; i < entityFields.Length; i++)
        {
            var entityField = entityFields[i];
            var value = entityField.Read(entity);

            entries[i + offset] = new HashEntry(entityField.Field, value);
        }
    }

    public static void ReadFieldsAndValues<TEntity>(this RedisEntityFields<TEntity> fields, RedisValue[] values, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (values == null) throw new ArgumentNullException(nameof(values));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        var entityFields = fields.EntityFields;
        if (values.Length < entityFields.Length * 2 + offset) throw new ArgumentOutOfRangeException(nameof(values));

        for (int i = 0; i < entityFields.Length; i++)
        {
            var entityField = entityFields[i];
            var value = entityField.Read(entity);

            values[offset++] = entityField.Field;
            values[offset++] = value;
        }
    }

    public static void ReadValues<TEntity>(this RedisEntityFields<TEntity> fields, RedisValue[] values, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (values == null) throw new ArgumentNullException(nameof(values));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        var entityFields = fields.EntityFields;
        if (values.Length < entityFields.Length + offset) throw new ArgumentOutOfRangeException(nameof(values));

        for (int i = 0; i < entityFields.Length; i++)
        {
            values[offset++] = entityFields[i].Read(entity);
        }
    }

    public static void ReadOddValues<TEntity>(this RedisEntityFields<TEntity> fields, RedisValue[] values, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (values == null) throw new ArgumentNullException(nameof(values));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        var entityFields = fields.EntityFields;
        if (values.Length < entityFields.Length * 2 + offset) throw new ArgumentOutOfRangeException(nameof(values));
        offset++;
        for (int i = 0; i < entityFields.Length; i++)
        {
            values[offset += 2] = entityFields[i].Read(entity);
        }
    }

    public static HashEntry[] GetEntries<TEntity>(this RedisEntityFields<TEntity> fields, TEntity entity)
    {
        var entityFields = fields.EntityFields;

        var entries = new HashEntry[entityFields.Length];

        for (int i = 0; i < entityFields.Length; i++)
        {
            var entityField = entityFields[i];

            var value = entityField.Read(entity);

            entries[i] = new HashEntry(entityField.Field, value);
        }

        return entries;
    }

    public static RedisValue[] GetFieldsAndValues<TEntity>(this RedisEntityFields<TEntity> fields, TEntity entity, int offset = 0)
    {
        var entityFields = fields.EntityFields;

        var values = new RedisValue[entityFields.Length * 2 + offset];

        for (int i = 0; i < entityFields.Length; i++)
        {
            var entityField = entityFields[i];
            var value = entityField.Read(entity);

            values[offset++] = entityField.Field;
            values[offset++] = value;
        }

        return values;
    }

    public static RedisValue[] GetValues<TEntity>(this RedisEntityFields<TEntity> fields, TEntity entity, int offset = 0)
    {
        var entityFields = fields.EntityFields;

        var values = new RedisValue[entityFields.Length + offset];

        for (int i = 0; i < entityFields.Length; i++)
        {
            values[offset++] = entityFields[i].Read(entity);
        }

        return values;
    }

    public static RedisValue[] GetEvenFields<TEntity>(this RedisEntityFields<TEntity> fields, int offset = 0)
    {
        var entityFields = fields.EntityFields;

        var values = new RedisValue[entityFields.Length * 2 + offset];

        for (int i = 0; i < entityFields.Length; i++)
        {
            values[offset += 2] = entityFields[i].Field;
        }

        return values;
    }

    public static bool Write<TEntity>(this RedisEntityFields<TEntity> fields, TEntity entity, RedisValue[] values)
    {
        var entityFields = fields.EntityFields;
        if (entityFields.Length != values.Length) throw new ArgumentOutOfRangeException(nameof(values));

        var writen = false;
        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (!value.IsNull)
            {
                entityFields[i].Write(entity, in value);
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

        var entityFields = fields.EntityFields;

        entityFields[i++].Write(entity, in value);

        for (; i < values.Length; i++)
        {
            value = values[i];
            if (!value.IsNull) entityFields[i].Write(entity, in value);
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

        var entityFields = fields.EntityFields;

        entityFields[i++].Write(entity, in value);

        for (; i < values.Length; i++)
        {
            value = values[i];
            if (!value.IsNull) entityFields[i].Write(entity, in value);
        }
        return entity;
    }

    public static TEntity? GetEntity<TEntity>(this RedisEntityFields<TEntity> fields, RedisValue[] values) where TEntity : new()
        => fields.GetEntity<TEntity, TEntity>(values);
}