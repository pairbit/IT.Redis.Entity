namespace StackExchange.Redis.Entity.Formatters;

public class IntPtrFormatter : NullableFormatter<IntPtr>
{
    public static readonly IntPtrFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref IntPtr value) => value = (IntPtr)(long)redisValue;

    public override RedisValue Serialize(in IntPtr value) => (long)value;
}