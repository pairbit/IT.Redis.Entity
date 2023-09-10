namespace StackExchange.Redis.Entity.Internal;

internal class RedisValueFormatterProxy : IRedisValueFormatter
{
    private readonly object _formatter;

    public RedisValueFormatterProxy(object formatter)
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public void Deserialize<T>(in RedisValue redisValue, ref T? value)
        => GetFormatter<T>().Deserialize(in redisValue, ref value);

    public RedisValue Serialize<T>(in T? value)
        => GetFormatter<T>().Serialize(in value);

    private IRedisValueFormatter<T> GetFormatter<T>() => (IRedisValueFormatter<T>)_formatter;
}