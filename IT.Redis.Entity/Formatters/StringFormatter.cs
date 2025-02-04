namespace IT.Redis.Entity.Formatters;

public class StringFormatter : IRedisValueFormatter<string?>
{
    public static readonly StringFormatter Default = new();

    public void Deserialize(in RedisValue redisValue, ref string? value)
    {
        if (redisValue.IsNull)
        {
            value = null;
        }
        else if (redisValue.IsNullOrEmpty)
        {
            value = string.Empty;
        }
        else
        {
            value = redisValue;
        }
    }

    public RedisValue Serialize(in string? value)
        => value == null ? RedisValue.EmptyString : value;
}