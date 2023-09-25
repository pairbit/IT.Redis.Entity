namespace IT.Redis.Entity.Formatters;

public class Int64Formatter : NullableFormatter<Int64>
{
    public static readonly Int64Formatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref Int64 value) 
        => value = (long)redisValue;

    public override RedisValue Serialize(in Int64 value) => value;
}