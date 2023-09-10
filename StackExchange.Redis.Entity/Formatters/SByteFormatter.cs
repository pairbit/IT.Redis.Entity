namespace StackExchange.Redis.Entity.Formatters;

public class SByteFormatter : NullableFormatter<SByte>
{
    public static readonly SByteFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref SByte value) => value = (sbyte)redisValue;

    public override RedisValue Serialize(in SByte value) => value;
}