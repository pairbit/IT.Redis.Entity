namespace StackExchange.Redis.Entity;

public static class xIRedisEntityReader
{
    public static HashEntry[] ReadFields<T>(this IRedisEntityReader<T> reader, T entity, RedisValue[] fields)
    {
        var entries = new HashEntry[fields.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            var field = fields[i];
            var value = reader.Read(entity, in field);

            entries[i] = new HashEntry(field, value);
        }

        return entries;
    }

    public static HashEntry[] ReadAllFields<T>(this IRedisEntityReader<T> reader, T entity) => reader.ReadFields(entity, reader.Fields.All);
}