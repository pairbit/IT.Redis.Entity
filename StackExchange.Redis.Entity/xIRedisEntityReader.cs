namespace StackExchange.Redis.Entity;

public static class xIRedisEntityReader
{
    public static void Read<T>(this IRedisEntityReader<T> reader, HashEntry[] entries, T entity, RedisValue[] fields)
    {
        if (entries.Length != fields.Length) throw new ArgumentOutOfRangeException(nameof(entries));

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            var value = reader.Read(entity, in field);

            entries[i] = new HashEntry(field, value);
        }
    }

    public static void Read<T>(this IRedisEntityReader<T> reader, HashEntry[] entries, T entity) => reader.Read(entries, entity, reader.Fields.All);

    public static HashEntry[] GetEntries<T>(this IRedisEntityReader<T> reader, T entity, RedisValue[] fields)
    {
        var entries = new HashEntry[fields.Length];

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            var value = reader.Read(entity, in field);

            entries[i] = new HashEntry(field, value);
        }

        return entries;
    }

    public static HashEntry[] GetEntries<T>(this IRedisEntityReader<T> reader, T entity) => reader.GetEntries(entity, reader.Fields.All);
}