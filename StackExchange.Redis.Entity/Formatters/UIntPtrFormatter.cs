namespace StackExchange.Redis.Entity.Formatters;

public class UIntPtrFormatter : NullableFormatter<UIntPtr>
{
    public static readonly UIntPtrFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref UIntPtr value) => value = (UIntPtr)(ulong)redisValue;

    public override RedisValue Serialize(in UIntPtr value) => (ulong)value;
}