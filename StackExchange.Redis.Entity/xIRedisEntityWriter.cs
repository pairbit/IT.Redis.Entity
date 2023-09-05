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
}