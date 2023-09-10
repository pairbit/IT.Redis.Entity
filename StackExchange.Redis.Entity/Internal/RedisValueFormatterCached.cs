namespace StackExchange.Redis.Entity.Internal;

internal class RedisValueFormatterCached : IRedisValueFormatter
{
    public void Deserialize<T>(in RedisValue redisValue, ref T? value)
    {
        var formatter = RedisValueFormatter.GetFormatter<T>();

        if (formatter != null) formatter.Deserialize(in redisValue, ref value);

        else RedisValueFormatter.Default.Deserialize(redisValue, ref value);
    }

    public RedisValue Serialize<T>(in T? value)
    {
        var formatter = RedisValueFormatter.GetFormatter<T>();

        if (formatter != null) return formatter.Serialize(in value);

        return RedisValueFormatter.Default.Serialize(in value);
    }
}