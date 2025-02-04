namespace IT.Redis.Entity.Formatters;

public class RedisValueFormatter : IStructFormatter<RedisValue>
{
    public static readonly RedisValueFormatter Default = new();

    public void Deserialize(in RedisValue redisValue, ref RedisValue value)
        => value = redisValue;

    public RedisValue Serialize(in RedisValue value)
        => value.IsNull ? RedisValue.EmptyString : value;

    public void Deserialize(in RedisValue redisValue, ref RedisValue? value)
        => value = redisValue;

    public RedisValue Serialize(in RedisValue? value)
    {
        if (value != null)
        {
            var val = value.Value;
            if (!val.IsNull) return val;
        }
        return RedisValue.EmptyString;
    }
}