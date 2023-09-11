namespace StackExchange.Redis.Entity.Formatters;

public class ByteFormatter : NullableFormatter<Byte>
{
    public static readonly ByteFormatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref Byte value) => value = checked((byte)(uint)redisValue);

    public override RedisValue Serialize(in Byte value) => (uint)value;
}