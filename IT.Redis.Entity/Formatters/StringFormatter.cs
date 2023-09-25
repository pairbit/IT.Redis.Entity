using IT.Redis.Entity.Internal;

namespace IT.Redis.Entity.Formatters;

public class StringFormatter : IRedisValueFormatter<string>
{
    public static readonly StringFormatter Default = new();

    public void Deserialize(in RedisValue redisValue, ref string? value)
        => value = redisValue == RedisValues.Zero ? (string?)null : redisValue;

    public RedisValue Serialize(in string? value)
        => value == null ? RedisValues.Zero : value;
}