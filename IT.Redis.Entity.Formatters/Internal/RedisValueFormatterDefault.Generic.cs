namespace IT.Redis.Entity.Internal;

internal class RedisValueFormatterDefault<T> : IRedisValueFormatter<T>
{
    public void Deserialize(in RedisValue redisValue, ref T? value) 
        => RedisValueFormatterRegistry.DefaultFormatter.Deserialize(in redisValue, ref value);

    public RedisValue Serialize(in T? value) 
        => RedisValueFormatterRegistry.DefaultFormatter.Serialize(in value);
}