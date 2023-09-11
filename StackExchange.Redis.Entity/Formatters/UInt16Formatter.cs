namespace StackExchange.Redis.Entity.Formatters;

public class UInt16Formatter : NullableFormatter<UInt16>
{
    public static readonly UInt16Formatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref UInt16 value) 
        => value = checked((ushort)(uint)redisValue);

    public override RedisValue Serialize(in UInt16 value) => (uint)value;
}