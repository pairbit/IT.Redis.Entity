namespace StackExchange.Redis.Entity.Formatters;

public class BooleanFormatter : NullableFormatter<Boolean>
{
    public static readonly BooleanFormatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref Boolean value) => value = (bool)redisValue;

    public override RedisValue Serialize(in Boolean value) => value;
}