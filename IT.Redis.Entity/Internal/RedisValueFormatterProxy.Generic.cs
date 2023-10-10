namespace IT.Redis.Entity.Internal;

internal class RedisValueFormatterProxy<T> : IRedisValueFormatter<T>
{
    private readonly IRedisValueFormatter _formatter;

    public RedisValueFormatterProxy(IRedisValueFormatter formatter)
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public void Deserialize(in RedisValue redisValue, ref T? value)
        => _formatter.Deserialize(in redisValue, ref value);

    public RedisValue Serialize(in T? value)
        => _formatter.Serialize(in value);
}