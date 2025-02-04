using System.Buffers;

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
            entries[offset++] = new HashEntry(field.RedisValue, field.Read(entity));
        }
    }

    public static void ReadFieldsAndValues<TEntity>(this RedisEntityField<TEntity>[] fields, RedisValue[] values, TEntity entity, int offset = 0)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (values == null) throw new ArgumentNullException(nameof(values));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        if (values.Length < (fields.Length << 1) + offset) throw new ArgumentOutOfRangeException(nameof(values));

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            values[offset++] = field.RedisValue;
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
        if (values.Length < (fields.Length << 1) + offset) throw new ArgumentOutOfRangeException(nameof(values));
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
            entries[offset++] = new HashEntry(field.RedisValue, field.Read(entity));
        }

        return entries;
    }

    public static RedisValue[] GetFieldsAndValues<TEntity>(this RedisEntityField<TEntity>[] fields, TEntity entity, int offset = 0)
    {
        var values = new RedisValue[(fields.Length << 1) + offset];

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            values[offset++] = field.RedisValue;
            values[offset++] = field.Read(entity);
        }

        return values;
    }

    public static RedisValue[] GetCountAndFieldsAndValues<TEntity>(this RedisEntityField<TEntity>[] fields, TEntity[] entities, int offset = 0)
    {
        var values = new RedisValue[entities.Length * fields.Length + fields.Length + 1 + offset];
        values[offset++] = fields.Length;
        for (var i = 0; i < fields.Length; i++)
        {
            values[offset++] = fields[i].RedisValue;
        }
        for (int e = 0; e < entities.Length; e++)
        {
            var entity = entities[e];
            for (int f = 0; f < fields.Length; f++)
            {
                values[offset++] = fields[f].Read(entity);
            }
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
        var values = new RedisValue[(fields.Length << 1) + offset];

        for (int i = 0; i < fields.Length; i++, offset += 2)
        {
            values[offset] = fields[i].RedisValue;
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

    public static TEntity? GetEntity<TEntity, IEntity>(this RedisEntityField<IEntity>[] fields, RedisValue[] values, Func<TEntity> getEntity, int offset = 0) where TEntity : IEntity
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
        var entity = getEntity();

        fields[i++].Write(entity, in value);

        for (; i < fields.Length; i++)
        {
            value = values[offset++];
            if (!value.IsNull) fields[i].Write(entity, in value);
        }
        return entity;
    }

    public static TEntity? GetEntity<TEntity>(this RedisEntityField<TEntity>[] fields, RedisValue[] values, Func<TEntity> getEntity, int offset = 0)
        => fields.GetEntity<TEntity, TEntity>(values, getEntity, offset);

    public static TEntity? GetEntity<TEntity, IEntity>(this RedisEntityField<IEntity>[] fields, RedisValue[] values, int offset = 0) where TEntity : IEntity, new()
        => fields.GetEntity(values, static () => new TEntity(), offset);

    public static TEntity? GetEntity<TEntity>(this RedisEntityField<TEntity>[] fields, RedisValue[] values, int offset = 0) where TEntity : new()
        => fields.GetEntity<TEntity, TEntity>(values, offset);

    public static TEntity?[] GetEntities<TEntity, IEntity>(this RedisEntityField<IEntity>[] fields, RedisValue[] values, Func<TEntity> getEntity, int offset = 0) where TEntity : IEntity
    {
        if (values.Length % fields.Length != 0) throw new ArgumentOutOfRangeException(nameof(values));

        var len = values.Length / fields.Length;
        if (len == 0) return [];

        var entities = new TEntity?[len];

        for (int i = 0; i < entities.Length; i++, offset += fields.Length)
        {
            entities[i] = fields.GetEntity(values, getEntity, offset);
        }

        return entities;
    }

    public static TEntity?[] GetEntities<TEntity>(this RedisEntityField<TEntity>[] fields, RedisValue[] values, Func<TEntity> getEntity, int offset = 0)
        => fields.GetEntities<TEntity, TEntity>(values, getEntity, offset);

    public static TEntity?[] GetEntities<TEntity, IEntity>(this RedisEntityField<IEntity>[] fields, RedisValue[] values, int offset = 0) where TEntity : IEntity, new()
        => fields.GetEntities(values, static () => new TEntity(), offset);

    public static TEntity?[] GetEntities<TEntity>(this RedisEntityField<TEntity>[] fields, RedisValue[] values, int offset = 0) where TEntity : new()
        => fields.GetEntities<TEntity, TEntity>(values, offset);

    #region Rent

    public static ArraySegment<TEntity?> RentEntities<TEntity, IEntity>(this RedisEntityField<IEntity>[] fields, RedisValue[] values, Func<TEntity> getEntity, int offset = 0, ArrayPool<TEntity?>? pool = null) where TEntity : IEntity
    {
        if (values.Length % fields.Length != 0) throw new ArgumentOutOfRangeException(nameof(values));

        var count = values.Length / fields.Length;
        if (count == 0) return new([]);

        var entities = pool == null ? ArrayPool<TEntity?>.Shared.Rent(count) : pool.Rent(count);

        for (int i = 0; i < count; i++, offset += fields.Length)
        {
            entities[i] = fields.GetEntity(values, getEntity, offset);
        }

        return new(entities, 0, count);
    }

    public static ArraySegment<TEntity?> RentEntities<TEntity>(this RedisEntityField<TEntity>[] fields, RedisValue[] values, Func<TEntity> getEntity, int offset = 0, ArrayPool<TEntity?>? pool = null)
        => fields.RentEntities<TEntity, TEntity>(values, getEntity, offset, pool);

    public static ArraySegment<TEntity?> RentEntities<TEntity, IEntity>(this RedisEntityField<IEntity>[] fields, RedisValue[] values, int offset = 0, ArrayPool<TEntity?>? pool = null) where TEntity : IEntity, new()
        => fields.RentEntities(values, static () => new TEntity(), offset, pool);

    public static ArraySegment<TEntity?> RentEntities<TEntity>(this RedisEntityField<TEntity>[] fields, RedisValue[] values, int offset = 0, ArrayPool<TEntity?>? pool = null) where TEntity : new()
        => fields.RentEntities<TEntity, TEntity>(values, offset, pool);

    public static ArraySegment<HashEntry> RentEntries<TEntity>(this RedisEntityField<TEntity>[] fields, TEntity entity, int offset = 0, ArrayPool<HashEntry>? pool = null)
    {
        var count = fields.Length + offset;
        var entries = pool == null ? ArrayPool<HashEntry>.Shared.Rent(count) : pool.Rent(count);

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            entries[offset++] = new HashEntry(field.RedisValue, field.Read(entity));
        }

        return new(entries, 0, count);
    }

    public static ArraySegment<RedisValue> RentFieldsAndValues<TEntity>(this RedisEntityField<TEntity>[] fields, TEntity entity, int offset = 0, ArrayPool<RedisValue>? pool = null)
    {
        var count = (fields.Length << 1) + offset;
        var values = pool == null ? ArrayPool<RedisValue>.Shared.Rent(count) : pool.Rent(count);

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            values[offset++] = field.RedisValue;
            values[offset++] = field.Read(entity);
        }

        return new(values, 0, count);
    }

    public static ArraySegment<RedisValue> RentCountAndFieldsAndValues<TEntity>(this RedisEntityField<TEntity>[] fields, TEntity[] entities, int offset = 0, ArrayPool<RedisValue>? pool = null)
    {
        var count = entities.Length * fields.Length + fields.Length + 1 + offset;
        var values = pool == null ? ArrayPool<RedisValue>.Shared.Rent(count) : pool.Rent(count);
        values[offset++] = fields.Length;
        for (var i = 0; i < fields.Length; i++)
        {
            values[offset++] = fields[i].RedisValue;
        }
        for (int e = 0; e < entities.Length; e++)
        {
            var entity = entities[e];
            for (int f = 0; f < fields.Length; f++)
            {
                values[offset++] = fields[f].Read(entity);
            }
        }
        return new(values, 0, count);
    }

    public static ArraySegment<RedisValue> RentValues<TEntity>(this RedisEntityField<TEntity>[] fields, TEntity entity, int offset = 0, ArrayPool<RedisValue>? pool = null)
    {
        var count = fields.Length + offset;
        var values = pool == null ? ArrayPool<RedisValue>.Shared.Rent(count) : pool.Rent(count);

        for (int i = 0; i < fields.Length; i++)
        {
            values[offset++] = fields[i].Read(entity);
        }

        return new(values, 0, count);
    }

    public static ArraySegment<RedisValue> RentEvenFields<TEntity>(this RedisEntityField<TEntity>[] fields, int offset = 0, ArrayPool<RedisValue>? pool = null)
    {
        var count = (fields.Length << 1) + offset;
        var values = pool == null ? ArrayPool<RedisValue>.Shared.Rent(count) : pool.Rent(count);

        for (int i = 0; i < fields.Length; i++, offset += 2)
        {
            values[offset] = fields[i].RedisValue;
        }

        return new(values, 0, count);
    }

    #endregion Rent
}