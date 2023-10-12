namespace IT.Redis.Entity.Internal;

internal class RedisValueFormatterNotRegistered : IRedisValueFormatter
{
    public void Deserialize<T>(in RedisValue redisValue, ref T? value) => throw Ex.FormatterNotRegistered(typeof(T));

    public RedisValue Serialize<T>(in T? value) => throw Ex.FormatterNotRegistered(typeof(T));
}