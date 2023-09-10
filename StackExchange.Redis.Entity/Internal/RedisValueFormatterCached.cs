namespace StackExchange.Redis.Entity.Internal;

internal class RedisValueFormatterCached : IRedisValueFormatter
{
    public void Deserialize<T>(in RedisValue redisValue, ref T? value)
    {
        var formatter = RedisValueFormatterRegistry.GetFormatter<T>();

        if (formatter != null) formatter.Deserialize(in redisValue, ref value);

        else RedisValueFormatterRegistry.Default.Deserialize(redisValue, ref value);
    }

    public RedisValue Serialize<T>(in T? value)
    {
        var formatter = RedisValueFormatterRegistry.GetFormatter<T>();

        if (formatter != null) return formatter.Serialize(in value);

        return RedisValueFormatterRegistry.Default.Serialize(in value);
    }
}