namespace IT.Redis.Entity;

public static class xIRedisEntityWriter
{
    public static bool Write<T>(this IRedisEntityWriter<T> writer, T entity, HashEntry[] entries)
    {
        for (int i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            writer.Write(entity, entry.Name, entry.Value);
        }
        return entries.Length != 0;
    }

    public static bool Write<T>(this IRedisEntityWriter<T> writer, T entity, RedisValue[] fields, RedisValue[] values)
    {
        if (fields.Length != values.Length) throw new ArgumentOutOfRangeException(nameof(values));

        var writen = false;
        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (!value.IsNull)
            {
                writer.Write(entity, in fields[i], in value);
                writen = true;
            }
        }
        return writen;
    }

    public static TEntity? GetEntity<TEntity, IEntity>(this IRedisEntityWriter<IEntity> writer, HashEntry[] entries) where TEntity : IEntity, new()
    {
        if (entries.Length == 0) return default;

        var entity = new TEntity();//TODO: Instance if value exists

        for (int i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            writer.Write(entity, entry.Name, entry.Value);
        }

        return entity;
    }

    public static TEntity? GetEntity<TEntity, IEntity>(this IRedisEntityWriter<IEntity> writer, RedisValue[] fields, RedisValue[] values) where TEntity : IEntity, new()
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

        writer.Write(entity, in fields[i++], in value);

        for (; i < values.Length; i++)
        {
            value = values[i];
            if (!value.IsNull) writer.Write(entity, in fields[i], in value);
        }
        return entity;
    }

    public static TEntity? GetEntity<TEntity, IEntity>(this IRedisEntityWriter<IEntity> writer, in RedisValue field, in RedisValue value) where TEntity : IEntity, new()
    {
        if (value.IsNull) return default;

        var entity = new TEntity();

        writer.Write(entity, in field, in value);

        return entity;
    }

    public static T? GetEntity<T>(this IRedisEntityWriter<T> writer, HashEntry[] entries) where T : new()
        => writer.GetEntity<T, T>(entries);

    public static T? GetEntity<T>(this IRedisEntityWriter<T> writer, RedisValue[] fields, RedisValue[] values) where T : new()
        => writer.GetEntity<T, T>(fields, values);

    public static T? GetEntity<T>(this IRedisEntityWriter<T> writer, in RedisValue field, in RedisValue value) where T : new()
        => writer.GetEntity<T, T>(field, value);

    public static T? GetEntity<T>(this IRedisEntityWriter<T> writer, RedisValue[] values) where T : new()
        => writer.GetEntity<T, T>(writer.Fields.All, values);
}