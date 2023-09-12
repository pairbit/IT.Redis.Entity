namespace StackExchange.Redis.Entity.Formatters;

public class UriFormatter : IRedisValueFormatter<Uri>
{
    public static readonly UriFormatter Default = new();

    public void Deserialize(in RedisValue redisValue, ref Uri? value)
        => value = redisValue.IsNullOrEmpty ? null : new Uri((string)redisValue!, UriKind.RelativeOrAbsolute);

    public RedisValue Serialize(in Uri? value)
        => value == null ? RedisValue.EmptyString : value.OriginalString;
}