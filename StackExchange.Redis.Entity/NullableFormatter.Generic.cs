namespace StackExchange.Redis.Entity;

public abstract class NullableFormatter<T> : IRedisValueFormatter<T?>, IRedisValueFormatter<T> where T : struct
{
    public void Deserialize(in RedisValue redisValue, ref T? value)
    {
        if (redisValue.IsNullOrEmpty)
        {
            value = null;
        }
        else
        {
            T valueNotNull = default;
            Deserialize(in redisValue, ref valueNotNull);
            value = valueNotNull;
        }
    }

    public abstract void Deserialize(in RedisValue redisValue, ref T value);

    public RedisValue Serialize(in T? value) => value.HasValue ? Serialize(value.Value) : RedisValue.EmptyString;

    public abstract RedisValue Serialize(in T value);
}