using StackExchange.Redis.Entity.Internal;

namespace StackExchange.Redis.Entity.Formatters;

public class StringFormatter : IRedisValueFormatter<string>
{
    public static readonly StringFormatter Default = new();

    public void Deserialize(in RedisValue redisValue, ref string? value)
        => value = redisValue == RedisValues.False ? (string?)null : redisValue;

    public RedisValue Serialize(in string? value)
        => value == null ? RedisValues.False : value;
}