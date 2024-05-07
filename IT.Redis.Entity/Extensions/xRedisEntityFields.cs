namespace IT.Redis.Entity;

public static class xRedisEntityFields
{
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
}