namespace IT.Redis.Entity.Formatters;

public class IntPtrFormatter : NullableFormatter<IntPtr>
{
    public static readonly IntPtrFormatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref IntPtr value) 
        => value = (IntPtr)(long)redisValue;

    public override RedisValue Serialize(in IntPtr value) => (long)value;
}