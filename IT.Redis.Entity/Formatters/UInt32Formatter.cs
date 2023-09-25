namespace IT.Redis.Entity.Formatters;

public class UInt32Formatter : NullableFormatter<UInt32>
{
    public static readonly UInt32Formatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref UInt32 value) 
        => value = (uint)redisValue;

    public override RedisValue Serialize(in UInt32 value) => value;
}