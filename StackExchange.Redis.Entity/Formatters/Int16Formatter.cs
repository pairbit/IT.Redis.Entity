namespace StackExchange.Redis.Entity.Formatters;

public class Int16Formatter : NullableFormatter<Int16>
{
    public static readonly Int16Formatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref Int16 value) => value = (short)redisValue;

    public override RedisValue Serialize(in Int16 value) => value;
}