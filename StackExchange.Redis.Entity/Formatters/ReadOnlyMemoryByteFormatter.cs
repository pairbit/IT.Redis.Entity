using StackExchange.Redis.Entity.Internal;

namespace StackExchange.Redis.Entity.Formatters;

public class ReadOnlyMemoryByteFormatter : NullableFormatter<ReadOnlyMemory<Byte>>
{
    public static readonly ReadOnlyMemoryByteFormatter Default = new();

    public override void Deserialize(in RedisValue redisValue, ref ReadOnlyMemory<Byte> value) => value = redisValue;

    public override RedisValue Serialize(in ReadOnlyMemory<Byte> value) => value;

    public override void Deserialize(in RedisValue redisValue, ref ReadOnlyMemory<byte>? value)
        => value = redisValue == RedisValues.Zero ? (ReadOnlyMemory<byte>?)null : redisValue;

    public override RedisValue Serialize(in ReadOnlyMemory<byte>? value)
        => value == null ? RedisValues.Zero : value.Value;
}