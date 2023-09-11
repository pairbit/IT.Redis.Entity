using StackExchange.Redis.Entity.Internal;

namespace StackExchange.Redis.Entity.Formatters;

public class RedisValueFormatter : NullableFormatter<RedisValue>
{
    public static readonly RedisValueFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref RedisValue value)
        => value = redisValue == RedisValues.Null ? RedisValue.Null : redisValue;

    public override RedisValue Serialize(in RedisValue value)
        => value.IsNull ? RedisValues.Null : value;

    public override void Deserialize(in RedisValue redisValue, ref RedisValue? value)
        => value = redisValue == RedisValues.Null ? (RedisValue?)null : redisValue;

    public override RedisValue Serialize(in RedisValue? value)
        => value == null || value.Value.IsNull ? RedisValues.Null : value.Value;
}