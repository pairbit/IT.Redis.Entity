namespace IT.Redis.Entity;

public static class xIRedisEntityReader
{
    public static void Read<T>(this IRedisEntityReader<T> reader, HashEntry[] entries, T entity, RedisValue[] fields, int offset = 0)
    {
        if (entries == null) throw new ArgumentNullException(nameof(entries));
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        if (entries.Length < fields.Length + offset) throw new ArgumentOutOfRangeException(nameof(entries));

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            var value = reader.Read(entity, in field);

            entries[i + offset] = new HashEntry(field, value);
        }
    }

    public static void Read<T>(this IRedisEntityReader<T> reader, HashEntry[] entries, T entity, int offset = 0)
        => reader.Read(entries, entity, reader.Fields.All, offset);

    public static RedisValue[] GetValues<T>(this IRedisEntityReader<T> reader, T entity, RedisValue[] fields)
    {
        var values = new RedisValue[fields.Length];

        for (int i = 0; i < fields.Length; i++)
        {
            values[i] = reader.Read(entity, in fields[i]);
        }

        return values;
    }

    public static RedisValue[] GetValues<T>(this IRedisEntityReader<T> reader, T entity) => reader.GetValues(entity, reader.Fields.All);

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

    public static RedisValue Serialize<TEntity, TField>(this IRedisEntityReader<TEntity> reader, in RedisValue field, in TField value)
        => reader.GetSerializer<TField>(in field).Serialize(in value);

    public static HashEntry SerializeToHashEntry<TEntity, TField>(this IRedisEntityReader<TEntity> reader, in RedisValue field, in TField value)
        => new(field, reader.GetSerializer<TField>(in field).Serialize(in value));
}