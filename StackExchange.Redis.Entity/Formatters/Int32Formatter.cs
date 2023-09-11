namespace StackExchange.Redis.Entity.Formatters;

public class Int32Formatter : NullableFormatter<Int32>
{
    public static readonly Int32Formatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref Int32 value) 
        => value = (int)redisValue;

    public override RedisValue Serialize(in Int32 value) => value;
}