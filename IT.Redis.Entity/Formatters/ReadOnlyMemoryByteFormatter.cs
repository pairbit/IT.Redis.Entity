namespace IT.Redis.Entity.Formatters;

public class ReadOnlyMemoryByteFormatter : IStructFormatter<ReadOnlyMemory<byte>>
{
    public static readonly ReadOnlyMemoryByteFormatter Default = new();

    public void Deserialize(in RedisValue redisValue, ref ReadOnlyMemory<byte> value)
        => value = redisValue;

    public RedisValue Serialize(in ReadOnlyMemory<byte> value) => value;

    public void Deserialize(in RedisValue redisValue, ref ReadOnlyMemory<byte>? value)
        => value = redisValue;

    public RedisValue Serialize(in ReadOnlyMemory<byte>? value)
        => value == null ? RedisValue.EmptyString : value.Value;
}