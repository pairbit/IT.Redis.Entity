namespace StackExchange.Redis.Entity.Formatters;

public class ReadOnlyMemoryByteFormatter : NullableFormatter<ReadOnlyMemory<Byte>>
{
    public static readonly ReadOnlyMemoryByteFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref ReadOnlyMemory<Byte> value) => value = redisValue;

    public override RedisValue Serialize(in ReadOnlyMemory<Byte> value) => value;
}