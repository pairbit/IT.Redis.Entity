namespace StackExchange.Redis.Entity.Formatters;

public class DoubleFormatter : NullableFormatter<Double>
{
    public static readonly DoubleFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref Double value) => value = (double)redisValue;

    public override RedisValue Serialize(in Double value) => value;
}