namespace IT.Redis.Entity.Formatters;

public class UInt64Formatter : NullableFormatter<UInt64>
{
    public static readonly UInt64Formatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref UInt64 value) 
        => value = (ulong)redisValue;

    public override RedisValue Serialize(in UInt64 value) => value;
}