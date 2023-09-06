namespace StackExchange.Redis.Entity;

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
        for (int i = 0; i < fields.Length; i++)
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

    public static T? Get<T>(this IRedisEntityWriter<T> writer, HashEntry[] entries) where T : new()
    {
        if (entries.Length == 0) return default;

        var entity = new T();

        for (int i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            writer.Write(entity, entry.Name, entry.Value);
        }

        return entity;
    }

    public static T? Get<T>(this IRedisEntityWriter<T> writer, RedisValue[] fields, RedisValue[] values) where T : new()
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
        var entity = new T();

        writer.Write(entity, in fields[i++], in value);

        for (; i < values.Length; i++)
        {
            value = values[i];
            if (!value.IsNull) writer.Write(entity, in fields[i], in value);
        }
        return entity;
    }
}