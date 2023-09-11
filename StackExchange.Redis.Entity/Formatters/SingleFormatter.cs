namespace StackExchange.Redis.Entity.Formatters;

public class SingleFormatter : NullableFormatter<Single>
{
    public static readonly SingleFormatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref Single value) => value = (float)redisValue;

    public override RedisValue Serialize(in Single value) => (float)value;
}