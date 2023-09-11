namespace StackExchange.Redis.Entity.Formatters;

public class GuidFormatter : NullableFormatter<Guid>
{
    public static readonly GuidFormatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref Guid value)
        => value = new Guid(((ReadOnlyMemory<byte>)redisValue).Span);

    public override RedisValue Serialize(in Guid value) => value.ToByteArray();
}